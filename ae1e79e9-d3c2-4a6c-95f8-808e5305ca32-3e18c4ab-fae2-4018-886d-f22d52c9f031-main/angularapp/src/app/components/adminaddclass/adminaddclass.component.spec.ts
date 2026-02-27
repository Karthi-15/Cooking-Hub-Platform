
import { AdminaddclassComponent } from './adminaddclass.component';

import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';


describe('AdminaddclassComponent', () => {
  let component: AdminaddclassComponent;
  let fixture: ComponentFixture<AdminaddclassComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdminaddclassComponent ],
      imports: [ReactiveFormsModule, RouterTestingModule, HttpClientTestingModule, FormsModule]

    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminaddclassComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  fit('Frontend_should_create_adminaddclass_component', () => {
    expect(component).toBeTruthy();
  });
  fit('Frontend_should_contain_create_new_cooking_class_heading_in_the_adminaddclass_component', () => {
    const componentHTML = fixture.debugElement.nativeElement.outerHTML;
    expect(componentHTML).toContain('Create New Cooking Class');
  });
});
