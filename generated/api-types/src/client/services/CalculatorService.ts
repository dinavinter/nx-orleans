/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
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
    ): CancelablePromise<number> {
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
