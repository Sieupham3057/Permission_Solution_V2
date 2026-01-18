# Naming conventions – ghi thẳng vào guideline
🔐 Claims
Concept	Claim
UserId	sub
Username	preferred_username
Permission	permission
Role	role
🔑 Permission format
{resource}.{action}
customers.read
customers.create
orders.update


lowercase

dot-separated

case-insensitive at runtime

✅ Checklist Production-ready (đã đạt)

✅ Không hard-code Keycloak
✅ Config driven (appsettings)
✅ Multi-environment safe
✅ Permission canonicalization
✅ Không phụ thuộc case
✅ Không đổi business authorization code

🔥 Gợi ý nâng cấp tiếp theo (nên làm)

1️⃣ Tách AddKeycloakAuthentication() extension
2️⃣ Permission registry (permissions.json)
3️⃣ CI check naming permission
4️⃣ Audience chuẩn cho từng API
5️⃣ Gateway validate token, service chỉ authorize