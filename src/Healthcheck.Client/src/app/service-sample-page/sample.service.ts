import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
@Injectable()
export class SampleService {

  constructor(private http: HttpClient) { }


   fetchSample(){
     return this.http.get(`/sitecore/api/ssc/sample/service`);
   }
}
