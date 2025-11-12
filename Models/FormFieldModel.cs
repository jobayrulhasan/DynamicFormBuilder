namespace DynamicFormBuilder.Models
{
    public class FormFieldModel
    {
        public int FieldID { get; set; }
        public string Label { get; set; }
        public string FieldType { get; set; } // text, number, dropdown, checkbox
    }
}
