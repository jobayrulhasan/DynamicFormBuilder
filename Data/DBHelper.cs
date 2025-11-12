using DynamicFormBuilder.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DynamicFormBuilder.Data
{
    public class DBHelper
    {
        private readonly string _connectionString;

        public DBHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Insert Form
        public async Task<int> InsertFormAsync(string title)
        {
            await using SqlConnection con = new SqlConnection(_connectionString);
            await using SqlCommand cmd = new SqlCommand("INSERT INTO Forms(FormTitle) VALUES(@FormTitle); SELECT SCOPE_IDENTITY();", con);
            cmd.Parameters.AddWithValue("@FormTitle", title);

            await con.OpenAsync();
            return Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        // Insert Field
        public async Task InsertFormFieldAsync(int formId, string label, string fieldType)
        {
            await using SqlConnection con = new SqlConnection(_connectionString);
            await using SqlCommand cmd = new SqlCommand(
                "INSERT INTO FormFields(FormID, Label, FieldType) VALUES(@FormID,@Label,@FieldType)", con);
            cmd.Parameters.AddWithValue("@FormID", formId);
            cmd.Parameters.AddWithValue("@Label", label);
            cmd.Parameters.AddWithValue("@FieldType", fieldType);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        // Get Form with Fields
        public async Task<FormModel> GetFormWithFieldsAsync(int formId)
        {
            FormModel form = new FormModel();
            form.Fields = new List<FormFieldModel>();

            await using SqlConnection con = new SqlConnection(_connectionString);
            await using SqlCommand cmd = new SqlCommand(
                "SELECT F.FormID, F.FormTitle, FF.FieldID, FF.Label, FF.FieldType " +
                "FROM Forms F " +
                "INNER JOIN FormFields FF ON F.FormID = FF.FormID " +
                "WHERE F.FormID=@FormID", con);
            cmd.Parameters.AddWithValue("@FormID", formId);

            await con.OpenAsync();
            await using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                form.FormID = reader.GetInt32(0);
                form.FormTitle = reader.GetString(1);

                form.Fields.Add(new FormFieldModel
                {
                    FieldID = reader.GetInt32(2),
                    Label = reader.GetString(3),
                    FieldType = reader.GetString(4)
                });
            }

            return form;
        }

        // Save Form Data
        public async Task SaveFormDataAsync(int formId, int fieldId, string value)
        {
            await using SqlConnection con = new SqlConnection(_connectionString);
            await using SqlCommand cmd = new SqlCommand(
                "INSERT INTO FormData(FormID, FieldID, UserValue) VALUES(@FormID,@FieldID,@UserValue)", con);
            cmd.Parameters.AddWithValue("@FormID", formId);
            cmd.Parameters.AddWithValue("@FieldID", fieldId);
            cmd.Parameters.AddWithValue("@UserValue", value);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<DataTable> GetAllFormsAsync()
        {
            DataTable dt = new DataTable();
            await using SqlConnection con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            using SqlCommand cmd = new SqlCommand("SELECT FormID, FormTitle, CreatedDate FROM Forms ORDER BY FormID DESC", con);
            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            return dt;
        }
    }
}