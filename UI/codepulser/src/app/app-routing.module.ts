import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CategoryListComponent } from './features/category/category-list/category-list.component';
import { AddCategoryComponent } from './features/category/add-category/add-category.component';
import {FormsModule} from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

const routes: Routes = [
  {path:"admin/categories",component: CategoryListComponent},
  {path : "admin/category/add",component: AddCategoryComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes),
    FormsModule,
    HttpClientModule
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
