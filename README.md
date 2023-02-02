## Example code for article Mobile Device Authentication (Poor Man’s Confidential Client)
https://www.biglittleendian.com/article/mobile-device-authentication

In this article, we discovered the joy of DIY token protection for our API endpoints with not just one, but multiple authentication schemes.
Microsoft has spoken, and they don't trust mobile apps to keep secrets safe. Updating my mobile code libraries from **ADAL** (Azure Active Directory Authentication Library) to **MSAL** (Microsoft Authentication Library) I got an exception thrown in my face, informing me that my once trusty mobile app could no longer be a **Confidential Client**.
