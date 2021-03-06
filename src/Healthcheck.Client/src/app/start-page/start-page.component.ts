import { Component, OnInit } from '@angular/core';
import { HealthcheckService } from '../healthcheck.service';
import { ComponentGroup, ComponentHealth } from '../contracts/component';
import * as Chart from 'chart.js'

@Component({
  selector: 'app-start-page',
  templateUrl: './start-page.component.html',
  styleUrls: ['./start-page.component.scss']
})
export class StartPageComponent implements OnInit {

  constructor(private healthcheckService: HealthcheckService) { }
  canvas: any;
  ctx: any;

  ngAfterViewInit() {
  }

  loadChart() {
    this.canvas = document.getElementById('myChart');
    this.ctx = this.canvas.getContext('2d');
    let myChart = new Chart(this.ctx, {
      type: 'pie',
      data: {
        labels: ["Healthy", "Warning", "Error", "Unknown"],
        datasets: [{
          label: '# of Components',
          data: [this.healthyCount, this.warningCount, this.errorCount, this.unknownCount],
          backgroundColor: [
            'rgba(0, 128, 0, 1)',
            'rgba(255, 166, 0, 1)',
            'rgba(255, 0, 0, 1)',
            'rgba(0, 0, 0, 1)'
          ],
          borderWidth: 1
        }]
      },
      options: {
        legend: {
          display: false
        }
      }
    });
  }

  isLoading: boolean;
  response: Array<ComponentGroup>;

  filterStates: any;
  selectedState: any;
  isTableView: boolean;
  viewType:any;
  memoryUsage:any;
  

  ngOnInit() {

    this.load();
    this.filterStates = ["All", "Healthy only", "Non-Healthy only"];
    this.selectedState = "All";
    this.isTableView = false;
    this.viewType = "normal";
    this.loadAppInformation();
    this.updateAppInformation();
  }

  appInfo: any;

  
  updateAppInformation() {
    setInterval(()=> {
      this.loadAppInformation();
      },2000); 
  }

  loadAppInformation(){
    this.healthcheckService.fetchApplicationInformation().subscribe({
      next: response => {
        this.appInfo = response;
        
      }, error: response => {
      }
    });
  }

  load() {
    this.isLoading = true;
    
    this.healthcheckService.fetchStatus().subscribe({
      next: response => {
        this.response = response as Array<ComponentGroup>;
        this.isLoading = false;
        this.flattenComponents();
        this.loadChart();
      }, error: response => {
      }
    });
  }

  run() {
    this.isLoading = true;
    this.healthcheckService.runHealthcheck().subscribe({
      next: response => {
        this.response = response as Array<ComponentGroup>;
        this.isLoading = false;
        this.flattenComponents();
        this.loadChart();
      }, error: response => {
      }
    });
  }

  healthyCount: any;
  warningCount: any;
  errorCount: any;
  unknownCount: any;
  flattenComponents() {
    this.healthyCount = 0;
    this.warningCount = 0;
    this.errorCount = 0;
    this.unknownCount = 0;
    for (var i = 0; i < this.response.length; i++) {
      for (var j = 0; j < this.response[i].Components.length; j++) {

        if (this.response[i].Components[j].Status == '' || this.response[i].Components[j].Status == 'Waiting') {
          this.unknownCount++;
          if (this.selectedState == this.filterStates[0]) {
            this.response[i].Components[j].Display = true;
          }
          else {
            this.response[i].Components[j].Display = false;
          }
        }

        if (this.response[i].Components[j].Status == 'Healthy') {
          this.healthyCount++;
          if (this.selectedState == this.filterStates[0] || this.selectedState == this.filterStates[1]) {
            this.response[i].Components[j].Display = true;
          }
          else {
            this.response[i].Components[j].Display = false;
          }
        }

        if (this.response[i].Components[j].Status == 'Warning') {
          this.warningCount++;

          if (this.selectedState == this.filterStates[0] || this.selectedState == this.filterStates[2]) {
            this.response[i].Components[j].Display = true;
          }
          else {
            this.response[i].Components[j].Display = false;
          }
        }

        if (this.response[i].Components[j].Status == 'Error') {
          this.errorCount++;
          if (this.selectedState == this.filterStates[0] || this.selectedState == this.filterStates[2]) {
            this.response[i].Components[j].Display = true;
          }
          else {
            this.response[i].Components[j].Display = false;
          }
        }
      }
    }
  }

  componentResponse: any;
  doubleClick(component: any) {
    component.isLoading = true;
    let onlyState=false;
    if(component.Status == "Waiting"){
      onlyState = true;
    }
    this.healthcheckService.fetchStatusByComponent(component.Id, onlyState).subscribe({
      next: response => {
        this.componentResponse = response;
        for (var i = 0; i < this.response.length; i++) {
          for (var j = 0; j < this.response[i].Components.length; j++) {
            if (this.response[i].Components[j].Id == component.Id) {
              this.response[i].Components[j] = this.componentResponse;
              component.isLoading = false;
              this.flattenComponents();
            }
          }
        }
        this.loadChart();
      }, error: response => {
      }
    });
  }

  changeState() {
    this.flattenComponents();
  }

  getClass(component: any) {
    if (component.Display) {
      return "col-md-2 flex-correction";
    }
    else {
      return "";
    }
  }

  getStateClass(component: any){
    if(component.Status == "Healthy"){
      return "healthy-state";
    }
    else if(component.Status == "Warning"){
      return "warning-state";
    }
    else if(component.Status == "Error"){
      return "error-state";
    }
    else{
      return "unknown-state";
    }
  }

  cleanErrors() {
    var r = confirm("Are you sure to remove error entries?");
    if (r == true) {
      this.isLoading = true;
      this.healthcheckService.clearErrors().subscribe({
        next: response => {
          this.response = response as Array<ComponentGroup>;
          this.isLoading = false;
          this.flattenComponents();
          this.loadChart();
        }, error: response => {
        }
      });
    }
  }
}
