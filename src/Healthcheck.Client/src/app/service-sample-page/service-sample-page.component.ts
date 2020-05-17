import { Component, OnInit } from '@angular/core';
import { SampleService } from './sample.service';
import { SampleDto } from './sampleDto';

@Component({
  selector: 'app-service-sample-page',
  templateUrl: './service-sample-page.component.html',
  styleUrls: ['./service-sample-page.component.scss']
})
export class ServiceSamplePageComponent implements OnInit {

  response: SampleDto;
  constructor(public service: SampleService) {
     
   }

  ngOnInit() {
    this.service.fetchSample().subscribe(
      {
        next: data => {
          this.response = data as SampleDto;
          console.log(data);
        },
        error: error => {
          this.response = error;
        }
      }
    );
  }

}
