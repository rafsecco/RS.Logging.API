import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { NewComponent } from './components/logging/new/new.component';
import { ListComponent } from './components/logging/list/list.component';
import { DetailComponent } from './components/detail/detail.component';
import { NavegationComponent } from './shared/navegation/navegation.component';
import { CommonModule } from '@angular/common';

@NgModule({
	declarations: [
		AppComponent,
		NewComponent,
		ListComponent,
		DetailComponent,
		NavegationComponent
	],
	imports: [
		BrowserModule,
		AppRoutingModule,
		ReactiveFormsModule,
		HttpClientModule,
		CommonModule
	],
	providers: [],
	bootstrap: [AppComponent]
})
export class AppModule { }
