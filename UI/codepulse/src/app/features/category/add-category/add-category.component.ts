import { Component } from '@angular/core';
import {FormsModule} from '@angular/forms';
import {AddCategoryRequest} from '../models/ add-category-request.model';
import {CategoryService} from '../services/category.service';
import {response} from 'express';

@Component({
  selector: 'app-add-category',
  standalone: true,
  imports: [
    FormsModule
  ],
  templateUrl: './add-category.component.html',
  styleUrl: './add-category.component.css'
})
export class AddCategoryComponent {

  model:AddCategoryRequest;
  constructor(private  categoryServices : CategoryService) {
    this.model= {
      name:'',
      urlHandel:''
    };
  }
  onFormSubmit(){
    // Call the CategoryService to add a new category
    this.categoryServices.addCategory(this.model).subscribe({
      next: () => {
        console.log('Category added successfully!');
        // Optionally, reset the form or navigate to another page
      },
      error: (err) => {
        console.error('Error adding category:', err);
        // Handle error, e.g., show a message to the user
      }
    });
  }
}
