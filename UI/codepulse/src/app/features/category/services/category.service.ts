import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import {AddCategoryRequest} from '../models/ add-category-request.model';

@Injectable({
  providedIn: 'root'  // Ensures that the service is available throughout the app
})
export class CategoryService {
  constructor(private http: HttpClient) {}

  // Method to add a new category
  addCategory(model: AddCategoryRequest): Observable<void> {
    return this.http.post<void>('https://localhost:7282/api/Categories', model);
  }
}
