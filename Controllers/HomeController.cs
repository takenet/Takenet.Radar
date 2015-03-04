using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using Takenet.Radar.Models;
using AttributeRouting.Web.Mvc;

namespace Takenet.Radar.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/
        public ActionResult Index()
        {
            string USERNAME = "arquitetura.takenet@gmail.com";
            string PASSWORD = "Takenet123";

            var viewModel = new RadarViewModel();

            var service = new SpreadsheetsService("MySpreadsheetIntegration-v1");
            service.setUserCredentials(USERNAME, PASSWORD);

            // Instantiate a SpreadsheetQuery object to retrieve spreadsheets.
            var query = new SpreadsheetQuery();
            // Make a request to the API and get all spreadsheets.
            SpreadsheetFeed feed = service.Query(query);


            var entry = feed.Entries.OrderByDescending(x => x.Published).FirstOrDefault(x => x.Title.Text.Contains("Radar")) as SpreadsheetEntry;
            var categories = entry.Worksheets.Entries.FirstOrDefault(x => x.Title.Text == "Categories") as WorksheetEntry;
            var entities = entry.Worksheets.Entries.FirstOrDefault(x => x.Title.Text == "Entries") as WorksheetEntry;

            viewModel.Categories = GetCategories(categories, service);
            viewModel.Entities = GetEntities(viewModel.Categories, entities, service);
            
            return View(viewModel);
        }

        // GET: /Home/
        [Route("versions/{version}")]
        public ActionResult IndexByVersion(string version)
        {
            string USERNAME = "arquitetura.takenet@gmail.com";
            string PASSWORD = "Takenet123";

            var viewModel = new RadarViewModel();
            viewModel.Version = version != null ? version.Replace('-', '/') : version;

            var service = new SpreadsheetsService("MySpreadsheetIntegration-v1");
            service.setUserCredentials(USERNAME, PASSWORD);

            // Instantiate a SpreadsheetQuery object to retrieve spreadsheets.
            var query = new SpreadsheetQuery();
            // Make a request to the API and get all spreadsheets.
            SpreadsheetFeed feed = service.Query(query);

            var entry = feed.Entries.FirstOrDefault(x => x.Title.Text.Equals("Radar de Tecnologias_" + viewModel.Version)) as SpreadsheetEntry;
            var categories = entry.Worksheets.Entries.FirstOrDefault(x => x.Title.Text == "Categories") as WorksheetEntry;
            var entities = entry.Worksheets.Entries.FirstOrDefault(x => x.Title.Text == "Entries") as WorksheetEntry;

            viewModel.Categories = GetCategories(categories, service);
            viewModel.Entities = GetEntities(viewModel.Categories, entities, service);

            return View("Index", viewModel);
        }

        private IEnumerable<Entity> GetEntities(IList<Category> categories, WorksheetEntry entities, SpreadsheetsService service)
        {
            List<Entity> result = new List<Entity>();
            // Define the URL to request the list feed of the worksheet.
            AtomLink listFeedLink = entities.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);

            // Fetch the list feed of the worksheet.
            var listQuery = new ListQuery(listFeedLink.HRef.ToString());
            ListFeed listFeed = service.Query(listQuery);

            // Iterate through each row, printing its cell values.
            foreach (ListEntry row in listFeed.Entries)
            {
                var entity = new Entity();
                foreach (ListEntry.Custom element in row.Elements)
                {
                    switch (element.LocalName)
                    {
                        case "id":
                            entity.Id = int.Parse(element.Value);
                            break;
                        case "tecnologia":
                            entity.Name = element.Value;
                            break;
                        case "idcategoria":
                            entity.Category = categories.FirstOrDefault(x => x.Id == int.Parse(element.Value));
                            break;
                        case "status":
                            {
                                Status status;
                                Enum.TryParse(element.Value, out status);
                                entity.Status = status;
                            }
                            break;
                        case "justificativa":
                            entity.Description = element.Value;
                            break;
                        case "top":
                            entity.Top = element.Value;
                            break;
                        case "left":
                            entity.Left = element.Value;
                            break;
                        case "type":
                            entity.Type = element.Value;
                            break;
                    }
                }
                result.Add(entity);
            }
            return result;
        }

        private static IList<Category> GetCategories(WorksheetEntry categories, SpreadsheetsService service)
        {
            var result = new List<Category>();
            // Define the URL to request the list feed of the worksheet.
            AtomLink listFeedLink = categories.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);

            // Fetch the list feed of the worksheet.
            var listQuery = new ListQuery(listFeedLink.HRef.ToString());
            ListFeed listFeed = service.Query(listQuery);

            // Iterate through each row, printing its cell values.
            foreach (ListEntry row in listFeed.Entries)
            {
                var category = new Category();
                foreach (ListEntry.Custom element in row.Elements)
                {
                    if (element.LocalName == "id")
                    {
                        category.Id = int.Parse(element.Value);
                        category.Quadrant = int.Parse(element.Value);
                    }
                    if (element.LocalName == "title")
                    {
                        category.Name = element.Value;
                    }
                }
                result.Add(category);
            }
            return result;
        }
    }
}
