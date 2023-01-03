using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HostServer.Pages
{
    public class TestModel : PageModel
    {
        private IConfigurationRoot? ConfigRoot;

        public TestModel(IConfiguration configRoot)
        {
            ConfigRoot = (IConfigurationRoot)configRoot;
        }

        public ContentResult OnGet()
        {
            string str = "";
            foreach (var provider in ConfigRoot!.Providers.ToList())
            {
                str += provider.ToString() + "\n";
            }

            return Content(str);
        }
    }
}
