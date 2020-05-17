import { TestBed, inject } from '@angular/core/testing';

import { HealthcheckService } from './healthcheck.service';

describe('HealthcheckService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [HealthcheckService]
    });
  });

  it('should be created', inject([HealthcheckService], (service: HealthcheckService) => {
    expect(service).toBeTruthy();
  }));
});
