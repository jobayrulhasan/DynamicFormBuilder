namespace DynamicFormBuilder.Models
{
    public class FormModel
    {
        public int FormID { get; set; }
        public string FormTitle { get; set; }
        public List<FormFieldModel> Fields { get; set; }
    }
}
