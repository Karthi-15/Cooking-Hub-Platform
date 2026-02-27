import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { UserviewclassComponent } from './userviewclass.component';

describe('UserviewclassComponent', () => {
  let component: UserviewclassComponent;
  let fixture: ComponentFixture<UserviewclassComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserviewclassComponent ],
      imports: [ReactiveFormsModule, RouterTestingModule, HttpClientTestingModule, FormsModule],

    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UserviewclassComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  fit('Frontend_should_create_userviewclass_component', () => {
    expect(component).toBeTruthy();
  });
  fit('Frontend_should_display_heading_available_classes_in_userviewclass_component', () => {
    const compiled = fixture.nativeElement;
    expect(compiled.textContent).toContain('Available Classes');
  });
});
