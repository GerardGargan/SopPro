namespace Backend.Service.Interface
{
    public interface ITemplateService
    {
        public Task<string> RenderTemplateAsync(string templateName, object model);

    }
}