import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class HealthcheckService {

  constructor(private httpClient:HttpClient) { }

  fetchStatus(){
    return this.httpClient.get('/sitecore/api/ssc/healthcheck/get?sc_site=shell');
  }

  runHealthcheck(){
    return this.httpClient.get('/sitecore/api/ssc/healthcheck/run?sc_site=shell');
  }

  fetchStatusByComponent(id:any){
    return this.httpClient.get('/sitecore/api/ssc/healthcheck/component?id='+id+'&sc_site=shell');
  }

  fetchApplicationInformation(){
    return this.httpClient.get('/sitecore/api/ssc/healthcheck/AppInfo?sc_site=shell');
  }

  clearErrors(){
    return this.httpClient.get('/sitecore/api/ssc/healthcheck/errors/clear?sc_site=shell');
  }
}
