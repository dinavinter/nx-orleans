/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import type { Observable } from 'rxjs';
import { BaseHttpRequest } from '../core/BaseHttpRequest';
@Injectable({
    providedIn: 'root',
})
export class CalculatorService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * @param l
     * @param r
     * @returns number Success
     * @throws ApiError
     */
    public add(
        l: number,
        r: number,
    ): Observable<number> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/Calculator/{l}+{r}',
            path: {
                'l': l,
                'r': r,
            },
            errors: {
                400: `Bad Request`,
            },
        });
    }
}
