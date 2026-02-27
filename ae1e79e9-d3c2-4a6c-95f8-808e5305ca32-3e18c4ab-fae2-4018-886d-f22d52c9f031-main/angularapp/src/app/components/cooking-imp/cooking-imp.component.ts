import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-cooking-imp',
  templateUrl: './cooking-imp.component.html',
  styleUrls: ['./cooking-imp.component.css']
})

export class CookingImpComponent implements OnInit {

  quote = {
    text: 'Cooking is an art, but an art requires knowing something about techniques and materials.',
    author: 'Nathan Myhrvold'
  };

  constructor() { }

  ngOnInit(): void { }
  
}