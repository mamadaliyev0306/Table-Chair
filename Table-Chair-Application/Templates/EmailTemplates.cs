namespace Table_Chair_Application.Templates
{
    public static class EmailTemplates
    {
        private static string _baseUrl = "https://sizning-saytingiz.com/unlock-account";

        public static void Configure(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public static string GetEmailVerificationTemplate(string name, string verificationUrl)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <title>Email manzilingizni tasdiqlang</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                        }}
                        .header {{
                            background-color: #4CAF50;
                            color: white;
                            padding: 20px;
                            text-align: center;
                            border-radius: 5px 5px 0 0;
                        }}
                        .content {{
                            padding: 20px;
                            background-color: #f9f9f9;
                            border-radius: 0 0 5px 5px;
                            border: 1px solid #ddd;
                            border-top: none;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 10px 20px;
                            background-color: #4CAF50;
                            color: white;
                            text-decoration: none;
                            border-radius: 5px;
                            margin-top: 15px;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 12px;
                            text-align: center;
                            color: #777;
                        }}
                    </style>
                </head>
                <body>
                    <div class='header'>
                        <h2>Email tasdiqlash</h2>
                    </div>
                    <div class='content'>
                        <p>Hurmatli {name},</p>
                        <p>Email manzilingizni tasdiqlash uchun quyidagi tugmani bosing:</p>
                        <div style='text-align: center;'>
                            <a href='{verificationUrl}' class='button'>Emailni tasdiqlash</a>
                        </div>
                        <p>Agar siz bu so'rovni yubormagan bo'lsangiz, bu xabarga e'tibor bermang.</p>
                        <p>Hurmat bilan,<br>Bizning jamoamiz</p>
                    </div>
                    <div class='footer'>
                        <p>© {DateTime.Now.Year} Bizning Kompaniya. Barcha huquqlar himoyalangan.</p>
                    </div>
                </body>
                </html>";
        }

        public static string GetPasswordResetTemplate(string name, string resetUrl)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <title>Parolni tiklash</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                        }}
                        .header {{
                            background-color: #2196F3;
                            color: white;
                            padding: 20px;
                            text-align: center;
                            border-radius: 5px 5px 0 0;
                        }}
                        .content {{
                            padding: 20px;
                            background-color: #f9f9f9;
                            border-radius: 0 0 5px 5px;
                            border: 1px solid #ddd;
                            border-top: none;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 10px 20px;
                            background-color: #2196F3;
                            color: white;
                            text-decoration: none;
                            border-radius: 5px;
                            margin-top: 15px;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 12px;
                            text-align: center;
                            color: #777;
                        }}
                    </style>
                </head>
                <body>
                    <div class='header'>
                        <h2>Parolni tiklash</h2>
                    </div>
                    <div class='content'>
                        <p>Hurmatli {name},</p>
                        <p>Parolingizni tiklash uchun quyidagi tugmani bosing:</p>
                        <div style='text-align: center;'>
                            <a href='{resetUrl}' class='button'>Parolni tiklash</a>
                        </div>
                        <p>Bu havola 24 soat davomida amal qiladi.</p>
                        <p>Agar siz bu so'rovni yubormagan bo'lsangiz, bu xabarga e'tibor bermang.</p>
                        <p>Hurmat bilan,<br>Bizning jamoamiz</p>
                    </div>
                    <div class='footer'>
                        <p>© {DateTime.Now.Year} Bizning Kompaniya. Barcha huquqlar himoyalangan.</p>
                    </div>
                </body>
                </html>";
        }

        public static string GetWelcomeTemplate(string name)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <title>Xush kelibsiz!</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                        }}
                        .header {{
                            background-color: #FF9800;
                            color: white;
                            padding: 20px;
                            text-align: center;
                            border-radius: 5px 5px 0 0;
                        }}
                        .content {{
                            padding: 20px;
                            background-color: #f9f9f9;
                            border-radius: 0 0 5px 5px;
                            border: 1px solid #ddd;
                            border-top: none;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 10px 20px;
                            background-color: #FF9800;
                            color: white;
                            text-decoration: none;
                            border-radius: 5px;
                            margin-top: 15px;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 12px;
                            text-align: center;
                            color: #777;
                        }}
                        ul {{
                            padding-left: 20px;
                        }}
                        li {{
                            margin-bottom: 10px;
                        }}
                    </style>
                </head>
                <body>
                    <div class='header'>
                        <h2>Xush kelibsiz!</h2>
                    </div>
                    <div class='content'>
                        <p>Hurmatli {name},</p>
                        <p>Bizning platformamizga a'zo bo'lganingiz uchun tashakkur!</p>
                        <p>Siz endi bizning barcha xizmatlarimizdan foydalanishingiz mumkin:</p>
                        <ul>
                            <li>Stollar va stullarni bron qilish</li>
                            <li>Buyurtmalaringizni boshqarish</li>
                            <li>Maxsus takliflar va chegirmalardan bahramand bo'lish</li>
                        </ul>
                        <div style='text-align: center;'>
                            <a href='{_baseUrl}/dashboard' class='button'>Kabinetga kirish</a>
                        </div>
                        <p>Yana bir bor xush kelibsiz!</p>
                        <p>Hurmat bilan,<br>Bizning jamoamiz</p>
                    </div>
                    <div class='footer'>
                        <p>© {DateTime.Now.Year} Bizning Kompaniya. Barcha huquqlar himoyalangan.</p>
                    </div>
                </body>
                </html>";
        }

        public static string GetAccountLockedTemplate(string name)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <title>Hisobingiz bloklandi</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                        }}
                        .header {{
                            background-color: #f44336;
                            color: white;
                            padding: 20px;
                            text-align: center;
                            border-radius: 5px 5px 0 0;
                        }}
                        .content {{
                            padding: 20px;
                            background-color: #f9f9f9;
                            border-radius: 0 0 5px 5px;
                            border: 1px solid #ddd;
                            border-top: none;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 10px 20px;
                            background-color: #f44336;
                            color: white;
                            text-decoration: none;
                            border-radius: 5px;
                            margin-top: 15px;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 12px;
                            text-align: center;
                            color: #777;
                        }}
                    </style>
                </head>
                <body>
                    <div class='header'>
                        <h2>Hisobingiz bloklandi</h2>
                    </div>
                    <div class='content'>
                        <p>Hurmatli {name},</p>
                        <p>Sizning hisobingiz xavfsizlik sabablari tufayli vaqtincha bloklandi.</p>
                        <p><strong>Sabab:</strong> Ko'p marta noto'g'ri parol kiritilgan.</p>
                        <p>Hisobingizni qayta faollashtirish uchun quyidagi tugmani bosing:</p>
                        <div style='text-align: center;'>
                            <a href='{_baseUrl}/unlock-account' class='button'>Hisobni qulflash</a>
                        </div>
                        <p>Agar bu harakat siz tomonidan amalga oshirilmagan bo'lsa, iltimos, darhol biz bilan bog'laning.</p>
                        <p>Hurmat bilan,<br>Bizning xavfsizlik jamoasi</p>
                    </div>
                    <div class='footer'>
                        <p>© {DateTime.Now.Year} Bizning Kompaniya. Barcha huquqlar himoyalangan.</p>
                    </div>
                </body>
                </html>";
        }

        public static string GetAccountUnlockedTemplate(string name)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <title>Hisobingiz qayta faollashtirildi</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                        }}
                        .header {{
                            background-color: #4CAF50;
                            color: white;
                            padding: 20px;
                            text-align: center;
                            border-radius: 5px 5px 0 0;
                        }}
                        .content {{
                            padding: 20px;
                            background-color: #f9f9f9;
                            border-radius: 0 0 5px 5px;
                            border: 1px solid #ddd;
                            border-top: none;
                        }}
                        .button {{
                            display: inline-block;
                            padding: 10px 20px;
                            background-color: #4CAF50;
                            color: white;
                            text-decoration: none;
                            border-radius: 5px;
                            margin-top: 15px;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 12px;
                            text-align: center;
                            color: #777;
                        }}
                    </style>
                </head>
                <body>
                    <div class='header'>
                        <h2>Hisobingiz qayta faollashtirildi</h2>
                    </div>
                    <div class='content'>
                        <p>Hurmatli {name},</p>
                        <p>Sizning hisobingiz qayta faollashtirildi. Endi tizimga kirishingiz mumkin.</p>
                        <div style='text-align: center;'>
                            <a href='{_baseUrl}/login' class='button'>Tizimga kirish</a>
                        </div>
                        <p>Agar bu harakat siz tomonidan amalga oshirilmagan bo'lsa, iltimos, darhol biz bilan bog'laning.</p>
                        <p>Hurmat bilan,<br>Bizning jamoamiz</p>
                    </div>
                    <div class='footer'>
                        <p>© {DateTime.Now.Year} Bizning Kompaniya. Barcha huquqlar himoyalangan.</p>
                    </div>
                </body>
                </html>";
        }

        internal static string GetOrderConfirmationTemplate(string name, string orderId, string orderUrl)
        {
            throw new NotImplementedException();
        }

        internal static string GetPasswordChangedTemplate(string name)
        {
            throw new NotImplementedException();
        }

        internal static string GetNewUserNotificationTemplate(string newUserEmail)
        {
            throw new NotImplementedException();
        }
    }
}