import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class HealthcheckService {

  constructor(private httpClient:HttpClient) { }

  fetchStatus(){
    return this.httpClient.get('/sitecore/api/ssc/healthcheck/get');
  }

  runHealthcheck(){
    return this.httpClient.get('/sitecore/api/ssc/healthcheck/run');
  }

  fetchStatusByComponent(id:any){
    return this.httpClient.get('/sitecore/api/ssc/healthcheck/component?id='+id);
  }
}
