import { Component } from '@angular/core';
import { AddCategoryRequest } from '../../models/ add-category-request.model';
import { CategoryService } from '../../services/category.service';

@Component({
  selector: 'app-add-category',
  templateUrl: './add-category.component.html',
  styleUrls: ['./add-category.component.css']
})
export class AddCategoryComponent {


  model:AddCategoryRequest;
  constructor(private  categoryServices : CategoryService) {
    this.model= {
      Name:'',
      UrlHandle:''
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
