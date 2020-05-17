import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceSamplePageComponent } from './service-sample-page.component';

describe('ServiceSamplePageComponent', () => {
  let component: ServiceSamplePageComponent;
  let fixture: ComponentFixture<ServiceSamplePageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceSamplePageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceSamplePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should be created', () => {
    expect(component).toBeTruthy();
  });
});
