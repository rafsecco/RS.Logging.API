import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { NewComponent } from './components/new/new.component';

const routes: Routes = [
	//{ path: '', redirectTo: '/new', pathMatch: 'full' },
	{ path: 'new', component: NewComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
