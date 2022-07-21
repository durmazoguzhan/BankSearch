import { Component, Inject} from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-personal-loan',
  templateUrl: './personal-loan.component.html',
  styleUrls: ['./personal-loan.component.css']
})
export class PersonalLoanComponent {

  public krediVadeleri: KrediVadesi[] = [];
  public personalLoans: PersonalLoan[] = [];
  public http!: HttpClient;
  @Inject('BASE_URL')
    baseUrl!: string;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http = http;
    this.baseUrl = baseUrl;
    this.GetLoanTerms();
    this.CalculatePersonalLoan('10000','3');
  }

  public GetLoanTerms(){
    this.http.get<KrediVadesi[]>(this.baseUrl + 'banksearch/kredi').subscribe(result => {
      this.krediVadeleri = result;
    }, error => console.error(error));
  }

  public CalculatePersonalLoan(krediTutari: string, krediVadesi: string) {
    this.http.get<PersonalLoan[]>(this.baseUrl + 'banksearch/kredi/hesapla/?krediTutari=' + krediTutari + '&krediVadesi=' + krediVadesi)
      .subscribe(result => {
        this.personalLoans = result;
      }, error => console.error(error));
    }
  }

interface PersonalLoan {
  bankaAdi: string;
  faizTutari: number;
  faiz: number;
  aylikTaksit: number;
  toplamOdeme: number;
  basvuruLinki: string;
}

interface KrediVadesi {
  krediVadeKodu: number;
  aySayisi: number;
}
