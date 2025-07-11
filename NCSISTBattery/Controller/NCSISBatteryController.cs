using CommonLibraryP.ShopfloorPKG;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NCSISTBattery.EFModel;
using NCSISTBattery.Services;
using SharedMod;
using Swashbuckle.AspNetCore.Annotations;

namespace NCSISTBattery.Controller
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class NCSISBatteryController : ControllerBase
    {
        private readonly DataService dataService;

        public NCSISBatteryController(DataService dataService)
        {
            this.dataService = dataService;
        }

        [SwaggerOperation(Summary = "🚧For Testing")]
        [HttpGet]
        public async Task<ActionResult<List<JigDTO>>> GetJigsStatusDTO()
        {
            var res = await dataService.GetAllJigsWithContentDTO();
            return Ok(res);
        }
        [SwaggerOperation(Summary = "🚧For Testing")]
        [HttpGet]
        public async Task<ActionResult<RecipeDTO>> GetCurrentRecipeDTO()
        {
            var res = await dataService.GetCurrentRecipeAndContentDTO();
            return res is not null ? Ok(res) : NotFound("No recipe selected yet");
        }
        [HttpGet]
        public async Task<ActionResult<CalculationDataDTO>> GetCalculationDataDTO()
        {
            var recipeRes = await dataService.GetCurrentRecipeAndContentDTO();
            if (recipeRes is null)
            {
                return NotFound("No recipe selected yet");
            }
            var calculationDataDTOs = await dataService.GetCalculationDataDTO();
            return Ok(calculationDataDTOs);
        }
    }
}
