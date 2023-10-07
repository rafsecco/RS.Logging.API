import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LoggingApiService } from 'src/app/shared/services/logging-api.service';
import { Log } from 'src/app/viewmodel/log';

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
			logLevel: ['', Validators.compose([Validators.required])],
			message: ['', Validators.compose([Validators.required])],
			stackTrace: ['',]
		})
	}

	submit(): void {
		if (this.formGroup.valid) {
			console.log(this.formGroup.value);
			console.log(this.log);
			this.log = Object.assign({}, this.log, this.formGroup.value);
			console.log(this.log);
			this.logService.postLogging(this.log).subscribe(res => {
				this.router.navigateByUrl("/");
			});
		}
	}

}
