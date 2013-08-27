using DotLiquid;
using FeintSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Site.Models;
using Site.Util;
using Newtonsoft.Json;

namespace Site.Controlers
{
	public class Tasks
	{	
		public static Response AddAll(Request request) {
			User user = User.GetLoggedUser (request.Session);
			if (user == null)
				return Response.Redirect("/login/");
			return add (request, user, Project.Find<Project>().Where().Execute()[0]);
		}


		public static Response Add(Request request) {
			User user = User.GetLoggedUser(request.Session);
			var project = Project.Ref<Project> (int.Parse(request.FormData ["project"])); //Find<Project> ().Where ().Eq ("Id", request.FormData ["project"]).Execute()[0];
			if (!project.isOwnerOfProject(user))
			{
				return null;
			}

			return add (request, user, project);
		}

		private static Response add (Request request, User user, Project project)
		{
			int priority = int.Parse (request.FormData ["priority"]);
			String message = request.FormData ["message"];
			int status = int.Parse (request.FormData ["status"]);
			DateTime deadline = DateTime.ParseExact (request.FormData ["deadline"], "dd/MM/yyyy", null);
			int projectId = int.Parse (request.FormData ["project"]);
			DateRegExpr dateRX = new DateRegExpr (message);
			if (dateRX.Success) {
				message = dateRX.Message;
				deadline = dateRX.Date;
			}
			PriorityRegExpr priorityRX = new PriorityRegExpr (message);
			if (priorityRX.Success) {
				message = priorityRX.Message;
				priority = priorityRX.Priority;
			}
			Task task;
			try {
				task = new Task () {
					Owner = user,
					ProjectToTask = project,
					Priority = priority,
					Message = message,
					Status = status,
					Updated = DateTime.Now,
					Created = DateTime.Now,
					Deadline = deadline
				};
				task.Save ();
				return new Response (JsonConvert.SerializeObject (new TaskSafe (task.Id, task.ProjectToTask.Value.Id, task.Priority, task.Message, task.Status, task.Deadline.ToString ("dd/MM/yyyy"), task.ProjectToTask.Value.Name)));
			}
			catch (Exception e) {
				return new Response (JsonConvert.SerializeObject (false));
			}
		}

//		public static Result updateAll(Long task) {
//			User user = Secured.getUser();
//			Long project_home = Project.getAllProjectsByUserId(user.id).get(0).id;
//			return update(project_home, task);
//		}
//
//		public static Result update(Long project,Long task) {
//			User user = Secured.getUser();
//			if (!Secured.isOwnerOfProject(project, user.id)) {
//				return forbidden();
//			}
//			Form<TaskForm> taskForm = form(TaskForm.class).bindFromRequest();
//			if (taskForm.hasErrors()) {
//				return badRequest();
//			}
//
//			// process task message
//			String message = taskForm.get().message;
//			String deadline = taskForm.get().deadline;
//			int priority = taskForm.get().priority;
//			Integer projectNew = taskForm.get().project;
//
//			DateRegExpr dateRX = new DateRegExpr(message);
//			if(dateRX.found()) {
//				message = dateRX.getMessage();
//				deadline = dateRX.getDate();
//			}
//
//			PriorityRegExpr priorityRX = new PriorityRegExpr(message);
//			if(priorityRX.found()) {
//				message = priorityRX.getMessage();
//				priority = priorityRX.getPriority();
//			}
//
//			Task taskR;
//			try {
//				taskR = Task.find.ref(task);
//				taskR.message=message;
//				taskR.deadline=new SimpleDateFormat("dd/MM/yyyy").parse(deadline);
//				taskR.priority=priority;
//				taskR.status=taskForm.get().status;
//				taskR.updated=new Date();
//
//				if ( projectNew != null ) {
//					taskR.project=Project.find.ref(Long.valueOf(projectNew));
//				}
//				taskR.save();
//
//				JsonNode result = Json.toJson(new TaskSafe(taskR.id, taskR.project.id, taskR.priority, taskR.message, taskR.status,new SimpleDateFormat("dd/MM/yyyy").format(taskR.deadline), taskR.project.name));
//				return ok(result);
//			} catch (ParseException e) {
//				JsonNode result = Json.toJson(Boolean.FALSE);
//				return ok(result);
//			}
//		}
//
//		public static Result deleteAll(Long task) {
//			User user = Secured.getUser();
//			Long project_home = Project.getAllProjectsByUserId(user.id).get(0).id;
//			return delete(project_home, task);
//		}
//
//		public static Result delete(Long project,Long task) {
//			Task taskR = Task.find.ref(task);
//			taskR.delete();
//			JsonNode result = Json.toJson(Boolean.TRUE);
//			return ok(result);
//		}
//
//		public static Result getWeek() {
//			User user = Secured.getUser();
//			// today    
//			Calendar date = new GregorianCalendar();
//			// reset hour, minutes, seconds and millis
//			date.set(Calendar.HOUR_OF_DAY, 0);
//			date.set(Calendar.MINUTE, 0);
//			date.set(Calendar.SECOND, 0);
//			date.set(Calendar.MILLISECOND, 0);
//			Date now = date.getTime();
//			// next day
//			date.add(Calendar.DAY_OF_MONTH, 7);
//			Date week = date.getTime();
//
//			List<Task> tasks= Task.find.where().eq("user.id", user.id).findList();
//
//			List<TaskSafe> tasksSafe= new ArrayList<TaskSafe>();
//			for(Task t : tasks){
//				if(t.deadline.compareTo(now) >= 0 && t.deadline.compareTo(week) <= 0) {
//					tasksSafe.add(new TaskSafe(t.id, t.project.id, t.priority, t.message, t.status, new SimpleDateFormat("dd/MM/yyyy").format(t.deadline), t.project.name));
//				}
//			}
//			JsonNode result = Json.toJson(tasksSafe);
//			return ok(result);
//		}
//
//		public static Result getAll() {
//			User user = Secured.getUser();
//
//			List<Task> tasks= Task.findAll(user.id);
//			List<TaskSafe> tasksSafe= new ArrayList<TaskSafe>();
//			for(Task t : tasks){
//				tasksSafe.add(new TaskSafe(t.id, t.project.id, t.priority, t.message, t.status,new SimpleDateFormat("dd/MM/yyyy").format(t.deadline), t.project.name));
//			}
//			JsonNode result = Json.toJson(tasksSafe);
//			return ok(result);
//		}
//
//		public static Result getByProject(Long project) {
//			User user = Secured.getUser();
//			if (!Secured.isOwnerOfProject(project, user.id)) {
//				return forbidden();
//			}
//			List<Task> tasks= Task.findByProject(project);
//			List<TaskSafe> tasksSafe= new ArrayList<TaskSafe>();
//			for(Task t : tasks){
//				tasksSafe.add(new TaskSafe(t.id, t.project.id, t.priority, t.message, t.status,new SimpleDateFormat("dd/MM/yyyy").format(t.deadline), t.project.name));
//			}
//			JsonNode result = Json.toJson(tasksSafe);
//			return ok(result);
//		}
//
//		public static Result getByUser() {
//			User user = Secured.getUser();
//			List<Task> tasks= Task.findByUser(user.id);
//			List<TaskSafe> tasksSafe= new ArrayList<TaskSafe>();
//			for(Task t : tasks){
//				tasksSafe.add(new TaskSafe(t.id, t.project.id, t.priority, t.message, t.status,new SimpleDateFormat("dd/MM/yyyy").format(t.deadline), t.project.name));
//			}
//			JsonNode result = Json.toJson(tasksSafe);
//			return ok(result);
//		}

	}
}

