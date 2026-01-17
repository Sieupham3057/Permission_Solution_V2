// body-class.service.ts
import { Injectable, Inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';

@Injectable({
    providedIn: 'root',
})
export class BodyClassService {
    constructor(@Inject(DOCUMENT) private document: Document) { }

    addClass(className: string) {
        this.document.body.classList.add(className);
    }

    removeClass(className: string) {
        this.document.body.classList.remove(className);
    }

    setClass(className: string) {
        this.document.body.className = className; // Replaces all classes on body
    }
}
