import { Component, Inject} from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-term-deposit',
  templateUrl: './term-deposit.component.html',
  styleUrls: ['./term-deposit.component.css']
})
export class TermDepositComponent{

  public mevduatVadeleri: MevduatVadesi[] = [];
  public termDeposits: TermDeposit[] = [];
  public http!: HttpClient;
  @Inject('BASE_URL')
  baseUrl!: string;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http = http;
    this.baseUrl = baseUrl;
    this.GetDepositTerms();
    this.CalculateTermDeposit('10000','32');
  }

  public GetDepositTerms() {
    this.http.get<MevduatVadesi[]>(this.baseUrl + 'banksearch/mevduat').subscribe(result => {
      this.mevduatVadeleri = result;
    }, error => console.error(error));
  }

  public CalculateTermDeposit(anapara: string, mevduatVadesi: string) {
    this.http.get<TermDeposit[]>(this.baseUrl + 'banksearch/mevduat/hesapla/?anapara=' + anapara + '&mevduatVadesi=' + mevduatVadesi)
      .subscribe(result => {
        this.termDeposits = result;
      }, error => console.error(error));
  }
}

interface TermDeposit {
  bankaAdi: string;
  netKazanc: number;
  faiz: number;
  vadeSonuTutar: number;
  basvuruLinki: string;
}

interface MevduatVadesi {
  mevduatVadeKodu: number;
  gunSayisi: number;
}
