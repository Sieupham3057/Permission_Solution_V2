# How can install and setup keycloak

## Setup Keycloak

- `docker-compose.yml`

        services:
        keycloak:
            image: quay.io/keycloak/keycloak:25.0
            container_name: keycloak
            command:
            - start-dev
            environment:
            KC_DB: postgres
            KC_DB_URL: jdbc:postgresql://postgres:5432/keycloak
            KC_DB_USERNAME: keycloak
            KC_DB_PASSWORD: keycloak
            KEYCLOAK_ADMIN: admin
            KEYCLOAK_ADMIN_PASSWORD: admin123
            KC_PROXY: edge
            ports:
            - "8080:8080"
            depends_on:
            - postgres
            restart: unless-stopped

        postgres:
            image: postgres:15
            container_name: keycloak_db
            environment:
            POSTGRES_DB: keycloak
            POSTGRES_USER: keycloak
            POSTGRES_PASSWORD: keycloak
            volumes:
            - kc_data:/var/lib/postgresql/data
            restart: unless-stopped

        volumes:
        kc_data:

Run command:   
- docker compose up -d
- http://localhost:8080/

## Setup keycloak as the following keyword.

 - realm: `company-dev`
 - client: `angular-spa`
 - Valid redirect URIs: `http://localhost:4200/*`
 - Web origins: `http://localhost:4200`
 - Client type: `OpenID Connect`
 - Client authentication: `OFF`
 - Authorization: `OFF`
 - Authentication flow: 
    + `[x] Standard flow` =>> For angular app
    + `[x] Direct access grants` => login from postman

 - `Save`
 - Create Role for client:
    `customers.read, customers.create, customer.update, customer.delete ...`
 - Create user: `Create user and input full info 'Username, Email, Firstname, Lastname'`

 - Role mapping => `Assigne role for user that mean grant permission`

    + Assign role: `Search client you want user access and the permission on that user like 'angular-spa customer.read, customer.create ..'`

 - CURL postman for test

        curl --location 'http://172.28.225.131:8080/realms/company-dev/protocol/openid-connect/token' \
        --header 'Content-Type: application/x-www-form-urlencoded' \
        --data-urlencode 'grant_type=password' \
        --data-urlencode 'client_id=angular-spa' \
        --data-urlencode 'username=USER_NAME' \
        --data-urlencode 'password=PASS_WORD' \
        --data-urlencode 'scope=openid'

 - Response Sample:

        {
            "access_token": "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI3Nk5ORTg3V2h6bHkzcGZZVXB1SndVS2lEM1JiclhvRGxxR1RDYVlNS29JIn0.eyJleHAiOjE3Njg3NDkwMjMsImlhdCI6MTc2ODc0ODcyMywianRpIjoiNDE2YTUxMWQtZGZjNi00NWMwLWJhYzktZTliN2JjNDA0OGViIiwiaXNzIjoiaHR0cDovLzE3Mi4yOC4yMjUuMTMxOjgwODAvcmVhbG1zL2NvbXBhbnktZGV2IiwiYXVkIjoiYWNjb3VudCIsInN1YiI6IjlmNWFjOTQ1LTEwMjMtNGE5NS05YjJkLTRmODE1M2RkNWY3ZSIsInR5cCI6IkJlYXJlciIsImF6cCI6ImFuZ3VsYXItc3BhIiwic2lkIjoiMzg4MzdmNDAtMGRjMS00ZDE0LTliMDItYmYzMTliMDI5N2Q2IiwiYWNyIjoiMSIsImFsbG93ZWQtb3JpZ2lucyI6WyJodHRwOi8vbG9jYWxob3N0OjQyMDAiXSwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbImRlZmF1bHQtcm9sZXMtY29tcGFueS1kZXYiLCJvZmZsaW5lX2FjY2VzcyIsInVtYV9hdXRob3JpemF0aW9uIl19LCJyZXNvdXJjZV9hY2Nlc3MiOnsiYW5ndWxhci1zcGEiOnsicm9sZXMiOlsiY3VzdG9tZXJzLmNyZWF0ZSIsIkN1c3RvbWVycy5SZWFkIl19LCJhY2NvdW50Ijp7InJvbGVzIjpbIm1hbmFnZS1hY2NvdW50IiwibWFuYWdlLWFjY291bnQtbGlua3MiLCJ2aWV3LXByb2ZpbGUiXX19LCJzY29wZSI6Im9wZW5pZCBwcm9maWxlIGVtYWlsIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJuYW1lIjoidXNlciAxIHVzZXIgMSIsInByZWZlcnJlZF91c2VybmFtZSI6InVzZXIxIiwiZ2l2ZW5fbmFtZSI6InVzZXIgMSIsImZhbWlseV9uYW1lIjoidXNlciAxIiwiZW1haWwiOiJ1c2VyMUB5b3BtYWlsLmNvbSJ9.O8guWVsr-H57CCCwhbn0gZAqO-b2FitY_3sO31jj7q0U0nV4n2mIavwWVN0Jr_xEa-_Bl7f0rYaclk3vPeYuOLhN3E4gH43FABFr-7j_lx8jWKeUXgd_-z2R-lUIyN9zuy4Er3-KvXMjtZYTFm05TapwMAwUJfytyeXpcuiKgcBEi_PC9HcweJTi4xiJo_gL7RQhNhQyHYFEkJTE4SE44_NTVShajqcaiSsWlKDuopKvVIGt6ioAG-xmYH-6-708e8GFl_iHwTenAgD1VpU2ZEiEQue3vPH3l5rv0cONQsq9VjxN_1Gj972qydVunWItPRqV1fbMGhmR3ECzXiQo6A",
            "expires_in": 300,
            "refresh_expires_in": 1800,
            "refresh_token": "eyJhbGciOiJIUzUxMiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI1ZjI4ZGRkZi01OGZmLTRiNGQtYTA3MC01YzkzMGQwMDQ1MTUifQ.eyJleHAiOjE3Njg3NTA1MjMsImlhdCI6MTc2ODc0ODcyMywianRpIjoiMmQwMWJkOTAtOGQyYy00ZWQyLTljY2EtMGVmYTUwMThiNjZkIiwiaXNzIjoiaHR0cDovLzE3Mi4yOC4yMjUuMTMxOjgwODAvcmVhbG1zL2NvbXBhbnktZGV2IiwiYXVkIjoiaHR0cDovLzE3Mi4yOC4yMjUuMTMxOjgwODAvcmVhbG1zL2NvbXBhbnktZGV2Iiwic3ViIjoiOWY1YWM5NDUtMTAyMy00YTk1LTliMmQtNGY4MTUzZGQ1ZjdlIiwidHlwIjoiUmVmcmVzaCIsImF6cCI6ImFuZ3VsYXItc3BhIiwic2lkIjoiMzg4MzdmNDAtMGRjMS00ZDE0LTliMDItYmYzMTliMDI5N2Q2Iiwic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSByb2xlcyB3ZWItb3JpZ2lucyBhY3IgYmFzaWMgZW1haWwifQ.1-jQPCJzJMtBAT1SbGBLA_k_zh4woXSyN6qzHCyKWNK7g61UFacoVj4BGqIEGjypqLLw6hKzdflGA2WdOwOfkA",
            "token_type": "Bearer",
            "id_token": "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI3Nk5ORTg3V2h6bHkzcGZZVXB1SndVS2lEM1JiclhvRGxxR1RDYVlNS29JIn0.eyJleHAiOjE3Njg3NDkwMjMsImlhdCI6MTc2ODc0ODcyMywianRpIjoiY2M3ZjY2ZjUtYjdmZC00YTg4LTk1MDctNDIxOGE5ZGJkY2Y4IiwiaXNzIjoiaHR0cDovLzE3Mi4yOC4yMjUuMTMxOjgwODAvcmVhbG1zL2NvbXBhbnktZGV2IiwiYXVkIjoiYW5ndWxhci1zcGEiLCJzdWIiOiI5ZjVhYzk0NS0xMDIzLTRhOTUtOWIyZC00ZjgxNTNkZDVmN2UiLCJ0eXAiOiJJRCIsImF6cCI6ImFuZ3VsYXItc3BhIiwic2lkIjoiMzg4MzdmNDAtMGRjMS00ZDE0LTliMDItYmYzMTliMDI5N2Q2IiwiYXRfaGFzaCI6Ik93V1locjdUOXc4MlhBY0ZRUTJlOVEiLCJhY3IiOiIxIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJuYW1lIjoidXNlciAxIHVzZXIgMSIsInByZWZlcnJlZF91c2VybmFtZSI6InVzZXIxIiwiZ2l2ZW5fbmFtZSI6InVzZXIgMSIsImZhbWlseV9uYW1lIjoidXNlciAxIiwiZW1haWwiOiJ1c2VyMUB5b3BtYWlsLmNvbSJ9.K1vZnmvYHOAp_2zCsCB1c_s4jXAup5RsUGfTDDd_yEx0HG9Ao73QMVAOS2o4zBx7-42oHbWXbHhgfskJ6baNahlndYi4MKD2n3Jf8oyHqaXGvGQEXT811ITAT4AoYS2a_J6uxaeCA2BNlm7Y7kx2jfK9ZMkVtWfSGru-kGSccwqc4ftNDMhLRG02M6fwIbO5zbqNEp0hdQ8PDKSLUV9OXaGsWDAtUlp1HMw2GCSje3_TAPgF0goI4BqYT8qbcw44LJ0aniCS4SVSC7YXYh7iIrnNVigi1MjFljd4ZycgNlk7GpgF0WjutbUpHt_-UxBH0PONhrhk1rBaDqBtkz7ncA",
            "not-before-policy": 0,
            "session_state": "38837f40-0dc1-4d14-9b02-bf319b0297d6",
            "scope": "openid profile email"
        }



