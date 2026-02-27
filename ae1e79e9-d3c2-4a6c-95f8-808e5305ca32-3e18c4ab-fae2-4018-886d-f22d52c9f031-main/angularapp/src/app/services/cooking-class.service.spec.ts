import { TestBed } from '@angular/core/testing';

import { CookingClassService } from './cooking-class.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('CookingClassService', () => {
  let service: CookingClassService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
    });
    service = TestBed.inject(CookingClassService);
  });

  fit('Frontend_should_create_cooking_class_service', () => {
    expect(service).toBeTruthy();
  });
});
