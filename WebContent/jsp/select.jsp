<%@ page language="java" contentType="text/html; charset=UTF-8"
    pageEncoding="UTF-8"%>
<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>Select</title>
</head>
<body>
<p>Select</p>
モードを選択してください。<br>
<from action="/home/HomeServlet"method="get">
1.注文　　　　　　　　　　　　　<input type="submit"value="決定"><br>
2.注文伝達　　　　　　　　　　<input type="submit"value="決定"><br>
3.管理者　　　　　　　　　　　　<input type="submit"value="決定"><br>
4.代金回収　　　　　　　　　　<input type="submit"value="決定"><br>
</body>
</html>