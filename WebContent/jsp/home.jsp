<%@ page language="java" contentType="text/html; charset=UTF-8"
    pageEncoding="UTF-8"%>
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>home</title>
</head>
<body>
<p>home</p>
IDとパスワードを入力してください。<br><br><br>
<from action="/home/HomeServlet"method="get">
ID<input type="text"><br><br><br>
Password<input type="text"><br>
</body>
</html>