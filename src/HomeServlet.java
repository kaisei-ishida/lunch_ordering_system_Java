import java.io.IOException;

import java.io.PrintWriter;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import javax.servlet.annotation.WebServlet;

@WebServlet("/HomeServlet")
public class HomeServlet extends HttpServlet{
	protected void doGet(HttpServletRequest request,
			HttpServletResponse response)
	        throws ServletException,IOException{
		response.setContentType("text/html;charset=UTF-8");
		PrintWriter out=response.getWriter();
		out.println("<html>");
		out.println("<title>Lunch_ordering_system_JaVa</title>");
		out.println("</html>");
		out.println("<h1>Lunch_ordering_System_Java</h1>");
		out.println("<p>1.注文</p>");
		out.println("<p>2.注文伝達係</p>");
		out.println("<p>3.管理者</p>");
		out.println("<p>4.代金回収</p>");
	}

}
