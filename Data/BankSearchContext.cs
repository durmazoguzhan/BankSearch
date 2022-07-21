using Microsoft.EntityFrameworkCore;
using BankSearch.Models;

namespace BankSearch.Data
{
	public class BankSearchContext:DbContext
	{
		//Modellerimiz ile veritabanımız arasındaki ilişkiyi yapılandırdığımız sınıf

		//program.cs'de girdiğimiz options-connectionstring yapısının injecton'ı
		public BankSearchContext(DbContextOptions<BankSearchContext>options)
			: base(options) { }

		// Modellerin EF-DbSet ile veritabanı tabloları ile injection'ı
		public DbSet<KrediVadesi> KrediVadeleri { get; set; }
		public DbSet<Kredi> Krediler { get; set; }
		public DbSet<MevduatVadesi> MevduatVadeleri { get; set; }
		public DbSet<Mevduat> Mevduatlar { get; set; }

	}
}
