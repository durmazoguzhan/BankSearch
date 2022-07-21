using Microsoft.AspNetCore.Mvc;
using BankSearch.Data;

namespace BankSearch.Controllers
{
	[Route("banksearch/[controller]")]
	[ApiController]
	public class KrediController : ControllerBase
	{
		IAppRepository _appRepository;

		public KrediController(IAppRepository appRepository)
		{
			_appRepository=appRepository;
		}

		[HttpGet]
		public ActionResult GetKrediVadeleri()
		{
			//Kredi vadelerinin çekilmesi
			return Ok(_appRepository.GetKrediVadeleri());
		}

		[HttpGet]
		[Route("hesapla")]
		public ActionResult CalculateKrediler(int krediTutari, int krediVadesi)
		{
			//alınan parametrelere göre ihtiyaç kredisi hesaplamalarının gerçeklenmesi
			if (krediTutari > 0 && krediVadesi > 0)
				return Ok(_appRepository.CalculateKrediler(krediTutari, krediVadesi));
			else
				return BadRequest();
		}
	}
}
