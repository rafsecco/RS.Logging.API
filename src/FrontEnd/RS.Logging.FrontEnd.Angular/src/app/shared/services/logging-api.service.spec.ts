import { TestBed } from '@angular/core/testing';

import { LoggingApiService } from './logging-api.service';

describe('LoggingApiService', () => {
  let service: LoggingApiService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LoggingApiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
