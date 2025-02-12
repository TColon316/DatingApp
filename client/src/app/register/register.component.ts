import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { Component, inject, OnInit, output } from '@angular/core';
import { DatePickerComponent } from '../_forms/date-picker/date-picker.component';
import { TextInputComponent } from '../_forms/text-input/text-input.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [DatePickerComponent, ReactiveFormsModule, TextInputComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent implements OnInit {
  // Input from Parent to Child component Logic
  // usersFromHomeComponent = input.required<any>();

  // Output from Child to Parent component Logic
  cancelRegister = output<boolean>();

  // Injected Services
  private accountService = inject(AccountService);
  private router = inject(Router);
  private formBuilder = inject(FormBuilder);
  registerForm: FormGroup = new FormGroup({});
  maxDate = new Date();
  validationErrors: string[] | undefined;

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm() {
    this.registerForm = this.formBuilder.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: [
        '',
        [Validators.required, Validators.minLength(4), Validators.maxLength(8)],
      ],
      confirmPassword: [
        '',
        [Validators.required, this.matchValues('password')],
      ],
    });

    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () =>
        this.registerForm.controls['confirmPassword'].updateValueAndValidity(),
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      // Checks to see if Child control (ConfirmPassword) matches to Parent control (Password)
      return control.value === control.parent?.get(matchTo)?.value
        ? null // Return NULL if it matches
        : { isMatching: true }; // isMatching is the name of the validator with a value of true to indicate a mismatch
    };
  }

  register() {
    const dob = this.getDateOnly(this.registerForm.get('dateOfBirth')?.value);

    this.registerForm.patchValue({ dateOfBirth: dob });

    this.accountService.register(this.registerForm.value).subscribe({
      next: (_) => this.router.navigateByUrl('/members'),
      error: (error) => (this.validationErrors = error),
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob: string | undefined) {
    if (!dob) return;

    return new Date(dob).toISOString().slice(0, 10);
  }
}
