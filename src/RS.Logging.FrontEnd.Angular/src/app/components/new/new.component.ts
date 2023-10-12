import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Log } from 'src/app/models/Log';
import { LogLevel } from 'src/app/models/LogLevel.enum';
import { LoggingApiService } from 'src/app/shared/services/logging-api.service';

@Component({
	selector: 'app-new',
	templateUrl: './new.component.html',
	styleUrls: ['./new.component.scss']
})
export class NewComponent {

	public formGroup: FormGroup;
	public log = {} as Log;

	constructor(
		private formBuilder: FormBuilder,
		private router: Router,
		private logService: LoggingApiService
	) {
		this.formGroup = this.formBuilder.group({
			logLevel: [0, Validators.compose([Validators.required, Validators.min(0), Validators.max(6)])],
			message: ['', Validators.compose([Validators.required])],
			stackTrace: [null,],
		});
	}

	public getLogLevelEnumValues() {
		return Object.values(LogLevel);
	}

	sendForm(): void {
		if (this.formGroup.valid) {
			//this.log = Object.assign({}, this.log, this.formGroup.value);
			//this.logService.postLogging(this.log).subscribe(res => {
			this.logService.postLogging(this.formGroup.value).subscribe(res => {
				this.router.navigateByUrl("/list");
			});
		}
	}

}
