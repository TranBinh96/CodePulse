// noinspection JSDeprecatedSymbols

import {RouterModule, Routes} from '@angular/router';
import {CategoryListComponent} from './features/category/category-list/category-list.component';
import {NgModule} from '@angular/core';
import {AddCategoryComponent} from './features/category/add-category/add-category.component';
import {FormsModule} from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';


export const routes: Routes = [
  {path:"admin/categories",component: CategoryListComponent},
  {path : "admin/category/add",component: AddCategoryComponent}
];


@NgModule({
  exports: [
    RouterModule

  ],
  imports: [
    RouterModule.forRoot(routes),
    FormsModule,
    HttpClientModule
  ]
})
export class AppRoutingModule { }

