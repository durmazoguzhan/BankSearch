using Microsoft.AspNetCore.Mvc;
using BankSearch.Data;

namespace BankSearch.Controllers
{
	[Route("banksearch/[controller]")]
	[ApiController]
	public class MevduatController : ControllerBase
	{
		IAppRepository _appRepository;

		public MevduatController(IAppRepository appRepository)
		{
			_appRepository = appRepository;
		}

		[HttpGet]
		public ActionResult GetMevduatVadeleri()
		{
			//mevduat vadelerinin çekilmesi
			return Ok(_appRepository.GetMevduatVadeleri());
		}

		[HttpGet]
		[Route("hesapla")]
		public ActionResult CalculateMevduatlar(int anapara, int mevduatVadesi)
		{
			//alınan parametrelere göre vadeli mevduat hesaplamalarının gerçeklenmesi
			if (anapara > 0 && mevduatVadesi > 0)
				return Ok(_appRepository.CalculateMevduatlar(anapara,mevduatVadesi));
			else
				return BadRequest();
		}
	}
}
