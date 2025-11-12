using DynamicFormBuilder.Data;
using DynamicFormBuilder.Models;
using Microsoft.AspNetCore.Mvc;

public class FormController : Controller
{
    private readonly DBHelper _dbHelper;

    public FormController(DBHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    // Step 1: Show Create Form Page
    public IActionResult CreateForm()
    {
        return View();
    }

    // Step 1: Save Form
    [HttpPost]
    public async Task<IActionResult> SaveForm(string FormTitle, List<FormFieldModel> Fields)
    {
        int formId = await _dbHelper.InsertFormAsync(FormTitle);

        foreach (var field in Fields)
        {
            await _dbHelper.InsertFormFieldAsync(formId, field.Label, field.FieldType);
        }

        return RedirectToAction("Index");
    }

    // Step 2: Preview Form
    public async Task<IActionResult> PreviewForm(int formId)
    {
        var form = await _dbHelper.GetFormWithFieldsAsync(formId);
        return View(form);
    }

    // Step 2: Submit Form Data
    [HttpPost]
    public async Task<IActionResult> SubmitForm(int formId, List<FormDataModel> Fields)
    {
        foreach (var field in Fields)
        {
            await _dbHelper.SaveFormDataAsync(formId, field.FieldID, field.UserValue);
        }

        return RedirectToAction("Success");
    }

    public IActionResult Success()
    {
        return View();
    }

    // ✅ List all forms
    public async Task<IActionResult> Index()
    {
        var forms = await _dbHelper.GetAllFormsAsync();
        return View(forms);
    }
}
