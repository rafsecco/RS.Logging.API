import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { NewComponent } from './components/new/new.component';
import { ListComponent } from './components/list/list.component';

const routes: Routes = [
	{ path: '', redirectTo: '/list', pathMatch: 'full' },
	{ path: 'new', component: NewComponent},
	{ path: 'list', component: ListComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
