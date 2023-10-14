import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { NewComponent } from './components/new/new.component';
import { ListComponent } from './components/list/list.component';
import { DetailComponent } from './components/detail/detail.component';

const routes: Routes = [
	{ path: '', redirectTo: '/list/1/2', pathMatch: 'full' },
	{ path: 'new', component: NewComponent},
	{ path: 'list/:page/:pageSize', component: ListComponent},
	{ path: 'detail/:id', component: DetailComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
