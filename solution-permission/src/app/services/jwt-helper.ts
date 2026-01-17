/**
 * Helper class to decode and find JWT expiration.
 */
import { Injectable } from '@angular/core';


@Injectable()
export class JwtHelper {

    public decodeJWT(token: string): any {
        // Split the token into its three parts (header, payload, signature)
        const [header, payload, signature] = token.split('.');

        // Decode the base64url encoded header and payload
        const decodeBase64Url = (str: string) => {
            // Replace base64url characters with base64 characters
            str = str.replace(/-/g, '+').replace(/_/g, '/');
            // Pad the string to make it a valid base64 string
            while (str.length % 4) {
                str += '=';
            }
            return JSON.parse(atob(str)); // Decode and parse the JSON
        };

        // Decode header and payload
        const decodedHeader = decodeBase64Url(header);
        const decodedPayload = decodeBase64Url(payload);

        return {
            header: decodedHeader,
            payload: decodedPayload,
            signature: signature
        };
    }

    // Function to convert a Unix timestamp to a human-readable date
    public convertUnixTimestampToDate(unixTimestamp: number): string {
        const date = new Date(unixTimestamp * 1000); // Convert seconds to milliseconds
        return date.toISOString(); // Return ISO 8601 format
    }

    // Extract the expiration date from the token
    getExpirationDate(token: string): string {
        const decodedJWT = this.decodeJWT(token);
        const expTimestamp = decodedJWT.payload.exp;
        return this.convertUnixTimestampToDate(expTimestamp);
    }

    // // Example JWT token
    // const jwtToken = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjhlN2I4MTRjLWUxZDYtNGJhOC0xZTg4LTA4ZGQ3MjZjZGI0MiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImFkbWluQGViZW5tb25uZXkuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6IkluYnVpbHQgQWRtaW5pc3RyYXRvciIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6ImFkbWluaXN0cmF0b3IiLCJwZXJtaXNzaW9uIjpbInVzZXJzLnZpZXciLCJ1c2Vycy5tYW5hZ2UiLCJyb2xlcy52aWV3Iiwicm9sZXMuYXNzaWduIiwicHJvZHVjdHMudmlldyIsInByb2R1Y3RzLm1hbmFnZSIsImN1c3RvbWVycy52aWV3IiwiY3VzdG9tZXJzLm1hbmFnZSJdLCJleHAiOjE3NDQwMTI0ODUsImlzcyI6Imh0dHA6Ly9nb29nbGUuY29tLnZuIiwiYXVkIjoiaHR0cDovL2dvb2dsZS5jb20udm4ifQ.l7RKGqeAdA6mvA978pcbfWXFwECE7QFfn4HUxqck_Ck";

    // // Decode the JWT
    // const decodedJWT = decodeJWT(jwtToken);
    // console.log(decodedJWT);

    //     // Extract expiration date
    //     const expirationDate = convertUnixTimestampToDate(decodedJWT.payload.exp);
    // console.log("Expiration Date:", expirationDate);

}
