//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.18.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;

#pragma warning disable 108 // Disable "CS0108 '{derivedDto}.ToJson()' hides inherited member '{dtoBase}.ToJson()'. Use the new keyword if hiding was intended."
#pragma warning disable 114 // Disable "CS0114 '{derivedDto}.RaisePropertyChanged(String)' hides inherited member 'dtoBase.RaisePropertyChanged(String)'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword."
#pragma warning disable 472 // Disable "CS0472 The result of the expression is always 'false' since a value of type 'Int32' is never equal to 'null' of type 'Int32?'
#pragma warning disable 1573 // Disable "CS1573 Parameter '...' has no matching param tag in the XML comment for ...
#pragma warning disable 1591 // Disable "CS1591 Missing XML comment for publicly visible type or member ..."
#pragma warning disable 8073 // Disable "CS8073 The result of the expression is always 'false' since a value of type 'T' is never equal to 'null' of type 'T?'"
#pragma warning disable 3016 // Disable "CS3016 Arrays as attribute arguments is not CLS-compliant"
#pragma warning disable 8603 // Disable "CS8603 Possible null reference return"

namespace MyNamespace
{
	using System = global::System;

	[System.CodeDom.Compiler.GeneratedCode("NSwag", "13.18.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0))")]
	public interface IPetStoreController
	{

		/// <summary>
		/// uploads an image
		/// </summary>

		/// <param name="petId">ID of pet to update</param>

		/// <param name="additionalMetadata">Additional data to pass to server</param>

		/// <param name="file">file to upload</param>

		/// <returns>successful operation</returns>

		System.Threading.Tasks.Task<ApiResponse> UploadFileAsync(long petId, string additionalMetadata, FileParameter file);

		/// <summary>
		/// Add a new pet to the store
		/// </summary>

		/// <param name="body">Pet object that needs to be added to the store</param>

		System.Threading.Tasks.Task AddPetAsync(Pet body);

		/// <summary>
		/// Update an existing pet
		/// </summary>

		/// <param name="body">Pet object that needs to be added to the store</param>

		System.Threading.Tasks.Task UpdatePetAsync(Pet body);

		/// <summary>
		/// Finds Pets by status
		/// </summary>

		/// <remarks>
		/// Multiple status values can be provided with comma separated strings
		/// </remarks>

		/// <param name="status">Status values that need to be considered for filter</param>

		/// <returns>successful operation</returns>

		System.Threading.Tasks.Task<System.Collections.Generic.ICollection<Pet>> FindPetsByStatusAsync(System.Collections.Generic.IEnumerable<Anonymous> status);

		/// <summary>
		/// Finds Pets by tags
		/// </summary>

		/// <remarks>
		/// Multiple tags can be provided with comma separated strings. Use tag1, tag2, tag3 for testing.
		/// </remarks>

		/// <param name="tags">Tags to filter by</param>

		/// <returns>successful operation</returns>

		[System.Obsolete]

		System.Threading.Tasks.Task<System.Collections.Generic.ICollection<Pet>> FindPetsByTagsAsync(System.Collections.Generic.IEnumerable<string> tags);

		/// <summary>
		/// Find pet by ID
		/// </summary>

		/// <remarks>
		/// Returns a single pet
		/// </remarks>

		/// <param name="petId">ID of pet to return</param>

		/// <returns>successful operation</returns>

		System.Threading.Tasks.Task<Pet> GetPetByIdAsync(long petId);

		/// <summary>
		/// Updates a pet in the store with form data
		/// </summary>

		/// <param name="petId">ID of pet that needs to be updated</param>

		/// <param name="name">Updated name of the pet</param>

		/// <param name="status">Updated status of the pet</param>

		System.Threading.Tasks.Task UpdatePetWithFormAsync(long petId, string name, string status);

		/// <summary>
		/// Deletes a pet
		/// </summary>


		/// <param name="petId">Pet id to delete</param>

		System.Threading.Tasks.Task DeletePetAsync(string api_key, long petId);

		/// <summary>
		/// Place an order for a pet
		/// </summary>

		/// <param name="body">order placed for purchasing the pet</param>

		/// <returns>successful operation</returns>

		System.Threading.Tasks.Task<Order> PlaceOrderAsync(Order body);

		/// <summary>
		/// Find purchase order by ID
		/// </summary>

		/// <remarks>
		/// For valid response try integer IDs with value &gt;= 1 and &lt;= 10. Other values will generated exceptions
		/// </remarks>

		/// <param name="orderId">ID of pet that needs to be fetched</param>

		/// <returns>successful operation</returns>

		System.Threading.Tasks.Task<Order> GetOrderByIdAsync(long orderId);

		/// <summary>
		/// Delete purchase order by ID
		/// </summary>

		/// <remarks>
		/// For valid response try integer IDs with positive integer value. Negative or non-integer values will generate API errors
		/// </remarks>

		/// <param name="orderId">ID of the order that needs to be deleted</param>

		System.Threading.Tasks.Task DeleteOrderAsync(long orderId);

		/// <summary>
		/// Returns pet inventories by status
		/// </summary>

		/// <remarks>
		/// Returns a map of status codes to quantities
		/// </remarks>

		/// <returns>successful operation</returns>

		System.Threading.Tasks.Task<System.Collections.Generic.IDictionary<string, int>> GetInventoryAsync();

		/// <summary>
		/// Creates list of users with given input array
		/// </summary>

		/// <param name="body">List of user object</param>

		/// <returns>successful operation</returns>

		System.Threading.Tasks.Task CreateUsersWithArrayInputAsync(System.Collections.Generic.IEnumerable<User> body);

		/// <summary>
		/// Creates list of users with given input array
		/// </summary>

		/// <param name="body">List of user object</param>

		/// <returns>successful operation</returns>

		System.Threading.Tasks.Task CreateUsersWithListInputAsync(System.Collections.Generic.IEnumerable<User> body);

		/// <summary>
		/// Get user by user name
		/// </summary>

		/// <param name="username">The name that needs to be fetched. Use user1 for testing.</param>

		/// <returns>successful operation</returns>

		System.Threading.Tasks.Task<User> GetUserByNameAsync(string username);

		/// <summary>
		/// Updated user
		/// </summary>

		/// <remarks>
		/// This can only be done by the logged in user.
		/// </remarks>

		/// <param name="username">name that need to be updated</param>

		/// <param name="body">Updated user object</param>

		System.Threading.Tasks.Task UpdateUserAsync(string username, User body);

		/// <summary>
		/// Delete user
		/// </summary>

		/// <remarks>
		/// This can only be done by the logged in user.
		/// </remarks>

		/// <param name="username">The name that needs to be deleted</param>

		System.Threading.Tasks.Task DeleteUserAsync(string username);

		/// <summary>
		/// Logs user into the system
		/// </summary>

		/// <param name="username">The user name for login</param>

		/// <param name="password">The password for login in clear text</param>

		/// <returns>successful operation</returns>

		System.Threading.Tasks.Task<string> LoginUserAsync(string username, string password);

		/// <summary>
		/// Logs out current logged in user session
		/// </summary>

		/// <returns>successful operation</returns>

		System.Threading.Tasks.Task LogoutUserAsync();

		/// <summary>
		/// Create user
		/// </summary>

		/// <remarks>
		/// This can only be done by the logged in user.
		/// </remarks>

		/// <param name="body">Created user object</param>

		/// <returns>successful operation</returns>

		System.Threading.Tasks.Task CreateUserAsync(User body);

	}

	[System.CodeDom.Compiler.GeneratedCode("NSwag", "13.18.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0))")]
	[Microsoft.AspNetCore.Mvc.Route("v2")]

	public partial class PetStoreController : Microsoft.AspNetCore.Mvc.ControllerBase
	{
		private IPetStoreController _implementation;

		public PetStoreController()
		{

		}

		/// <summary>
		/// uploads an image
		/// </summary>
		/// <param name="petId">ID of pet to update</param>
		/// <param name="additionalMetadata">Additional data to pass to server</param>
		/// <param name="file">file to upload</param>
		/// <returns>successful operation</returns>
		[Microsoft.AspNetCore.Mvc.HttpPost, Microsoft.AspNetCore.Mvc.Route("pet/{petId}/uploadImage")]
		public System.Threading.Tasks.Task<ApiResponse> UploadFile(long petId, string additionalMetadata, FileParameter file)
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("UploadFileAsync", new object[] { petId, additionalMetadata, file });
			return _implementation.UploadFileAsync(petId, additionalMetadata, file);
		}

		/// <summary>
		/// Add a new pet to the store
		/// </summary>
		/// <param name="body">Pet object that needs to be added to the store</param>
		[Microsoft.AspNetCore.Mvc.HttpPost, Microsoft.AspNetCore.Mvc.Route("pet")]
		public System.Threading.Tasks.Task AddPet([Microsoft.AspNetCore.Mvc.FromBody] Pet body)
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("AddPetAsync", new object[] { body });
			return _implementation.AddPetAsync(body);
		}

		/// <summary>
		/// Update an existing pet
		/// </summary>
		/// <param name="body">Pet object that needs to be added to the store</param>
		[Microsoft.AspNetCore.Mvc.HttpPut, Microsoft.AspNetCore.Mvc.Route("pet")]
		public System.Threading.Tasks.Task UpdatePet([Microsoft.AspNetCore.Mvc.FromBody] Pet body)
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("UpdatePetAsync", new object[] { body });
			return _implementation.UpdatePetAsync(body);
		}

		/// <summary>
		/// Finds Pets by status
		/// </summary>
		/// <remarks>
		/// Multiple status values can be provided with comma separated strings
		/// </remarks>
		/// <param name="status">Status values that need to be considered for filter</param>
		/// <returns>successful operation</returns>
		[Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("pet/findByStatus")]
		public System.Threading.Tasks.Task<System.Collections.Generic.ICollection<Pet>> FindPetsByStatus([Microsoft.AspNetCore.Mvc.FromQuery] System.Collections.Generic.IEnumerable<Anonymous> status)
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("FindPetsByStatusAsync", new object[] { status });
			return _implementation.FindPetsByStatusAsync(status);
		}

		/// <summary>
		/// Finds Pets by tags
		/// </summary>
		/// <remarks>
		/// Multiple tags can be provided with comma separated strings. Use tag1, tag2, tag3 for testing.
		/// </remarks>
		/// <param name="tags">Tags to filter by</param>
		/// <returns>successful operation</returns>
		[System.Obsolete]
		[Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("pet/findByTags")]
		public System.Threading.Tasks.Task<System.Collections.Generic.ICollection<Pet>> FindPetsByTags([Microsoft.AspNetCore.Mvc.FromQuery] System.Collections.Generic.IEnumerable<string> tags)
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("FindPetsByTagsAsync", new object[] { tags });
			return _implementation.FindPetsByTagsAsync(tags);
		}

		/// <summary>
		/// Find pet by ID
		/// </summary>
		/// <remarks>
		/// Returns a single pet
		/// </remarks>
		/// <param name="petId">ID of pet to return</param>
		/// <returns>successful operation</returns>
		[Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("pet/{petId}")]
		public System.Threading.Tasks.Task<Pet> GetPetById(long petId)
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("GetPetByIdAsync", new object[] { petId });
			return _implementation.GetPetByIdAsync(petId);
		}

		/// <summary>
		/// Updates a pet in the store with form data
		/// </summary>
		/// <param name="petId">ID of pet that needs to be updated</param>
		/// <param name="name">Updated name of the pet</param>
		/// <param name="status">Updated status of the pet</param>
		[Microsoft.AspNetCore.Mvc.HttpPost, Microsoft.AspNetCore.Mvc.Route("pet/{petId}")]
		public System.Threading.Tasks.Task UpdatePetWithForm(long petId, string name, string status)
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("UpdatePetWithFormAsync", new object[] { petId, name, status });
			return _implementation.UpdatePetWithFormAsync(petId, name, status);
		}

		/// <summary>
		/// Deletes a pet
		/// </summary>
		/// <param name="petId">Pet id to delete</param>
		[Microsoft.AspNetCore.Mvc.HttpDelete, Microsoft.AspNetCore.Mvc.Route("pet/{petId}")]
		public System.Threading.Tasks.Task DeletePet([Microsoft.AspNetCore.Mvc.FromHeader] string api_key, long petId)
		{
			Microsoft.AspNetCore.Http.HttpRequest.
			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("DeletePetAsync", new object[] { api_key, petId });
			return _implementation.DeletePetAsync(api_key, petId);
		}

		/// <summary>
		/// Place an order for a pet
		/// </summary>
		/// <param name="body">order placed for purchasing the pet</param>
		/// <returns>successful operation</returns>
		[Microsoft.AspNetCore.Mvc.HttpPost, Microsoft.AspNetCore.Mvc.Route("store/order")]
		public System.Threading.Tasks.Task<Order> PlaceOrder([Microsoft.AspNetCore.Mvc.FromBody] Order body)
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("PlaceOrderAsync", new object[] { body });
			return _implementation.PlaceOrderAsync(body);
		}

		/// <summary>
		/// Find purchase order by ID
		/// </summary>
		/// <remarks>
		/// For valid response try integer IDs with value &gt;= 1 and &lt;= 10. Other values will generated exceptions
		/// </remarks>
		/// <param name="orderId">ID of pet that needs to be fetched</param>
		/// <returns>successful operation</returns>
		[Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("store/order/{orderId}")]
		public System.Threading.Tasks.Task<Order> GetOrderById(long orderId)
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("GetOrderByIdAsync", new object[] { orderId });
			return _implementation.GetOrderByIdAsync(orderId);
		}

		/// <summary>
		/// Delete purchase order by ID
		/// </summary>
		/// <remarks>
		/// For valid response try integer IDs with positive integer value. Negative or non-integer values will generate API errors
		/// </remarks>
		/// <param name="orderId">ID of the order that needs to be deleted</param>
		[Microsoft.AspNetCore.Mvc.HttpDelete, Microsoft.AspNetCore.Mvc.Route("store/order/{orderId}")]
		public System.Threading.Tasks.Task DeleteOrder(long orderId)
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("DeleteOrderAsync", new object[] { orderId });
			return _implementation.DeleteOrderAsync(orderId);
		}

		/// <summary>
		/// Returns pet inventories by status
		/// </summary>
		/// <remarks>
		/// Returns a map of status codes to quantities
		/// </remarks>
		/// <returns>successful operation</returns>
		[Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("store/inventory")]
		public System.Threading.Tasks.Task<System.Collections.Generic.IDictionary<string, int>> GetInventory()
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("GetInventoryAsync", new object[] { });
			return _implementation.GetInventoryAsync();
		}

		/// <summary>
		/// Creates list of users with given input array
		/// </summary>
		/// <param name="body">List of user object</param>
		/// <returns>successful operation</returns>
		[Microsoft.AspNetCore.Mvc.HttpPost, Microsoft.AspNetCore.Mvc.Route("user/createWithArray")]
		public System.Threading.Tasks.Task CreateUsersWithArrayInput([Microsoft.AspNetCore.Mvc.FromBody] System.Collections.Generic.IEnumerable<User> body)
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("CreateUsersWithArrayInputAsync", new object[] { body });
			return _implementation.CreateUsersWithArrayInputAsync(body);
		}

		/// <summary>
		/// Creates list of users with given input array
		/// </summary>
		/// <param name="body">List of user object</param>
		/// <returns>successful operation</returns>
		[Microsoft.AspNetCore.Mvc.HttpPost, Microsoft.AspNetCore.Mvc.Route("user/createWithList")]
		public System.Threading.Tasks.Task CreateUsersWithListInput([Microsoft.AspNetCore.Mvc.FromBody] System.Collections.Generic.IEnumerable<User> body)
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("CreateUsersWithListInputAsync", new object[] { body });
			return _implementation.CreateUsersWithListInputAsync(body);
		}

		/// <summary>
		/// Get user by user name
		/// </summary>
		/// <param name="username">The name that needs to be fetched. Use user1 for testing.</param>
		/// <returns>successful operation</returns>
		[Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("user/{username}")]
		public System.Threading.Tasks.Task<User> GetUserByName(string username)
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("GetUserByNameAsync", new object[] { username });
			return _implementation.GetUserByNameAsync(username);
		}

		/// <summary>
		/// Updated user
		/// </summary>
		/// <remarks>
		/// This can only be done by the logged in user.
		/// </remarks>
		/// <param name="username">name that need to be updated</param>
		/// <param name="body">Updated user object</param>
		[Microsoft.AspNetCore.Mvc.HttpPut, Microsoft.AspNetCore.Mvc.Route("user/{username}")]
		public System.Threading.Tasks.Task UpdateUser(string username, [Microsoft.AspNetCore.Mvc.FromBody] User body)
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("UpdateUserAsync", new object[] { username, body });
			return _implementation.UpdateUserAsync(username, body);
		}

		/// <summary>
		/// Delete user
		/// </summary>
		/// <remarks>
		/// This can only be done by the logged in user.
		/// </remarks>
		/// <param name="username">The name that needs to be deleted</param>
		[Microsoft.AspNetCore.Mvc.HttpDelete, Microsoft.AspNetCore.Mvc.Route("user/{username}")]
		public System.Threading.Tasks.Task DeleteUser(string username)
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("DeleteUserAsync", new object[] { username });
			return _implementation.DeleteUserAsync(username);
		}

		/// <summary>
		/// Logs user into the system
		/// </summary>
		/// <param name="username">The user name for login</param>
		/// <param name="password">The password for login in clear text</param>
		/// <returns>successful operation</returns>
		[Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("user/login")]
		public System.Threading.Tasks.Task<string> LoginUser([Microsoft.AspNetCore.Mvc.FromQuery] string username, [Microsoft.AspNetCore.Mvc.FromQuery] string password)
		{
			var p = HostX.Core.PetStoreControllerNS.RedirectCalls.CreateParameter(new string[] { "header" "username", "password" }, new object[] { this.Request.Headers, username, password });
			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("LoginUserAsync", p);
			return _implementation.LoginUserAsync(username, password);
		}

		/// <summary>
		/// Logs out current logged in user session
		/// </summary>
		/// <returns>successful operation</returns>
		[Microsoft.AspNetCore.Mvc.HttpGet, Microsoft.AspNetCore.Mvc.Route("user/logout")]
		public System.Threading.Tasks.Task LogoutUser()
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("LogoutUserAsync", new object[] { });
			return _implementation.LogoutUserAsync();
		}

		/// <summary>
		/// Create user
		/// </summary>
		/// <remarks>
		/// This can only be done by the logged in user.
		/// </remarks>
		/// <param name="body">Created user object</param>
		/// <returns>successful operation</returns>
		[Microsoft.AspNetCore.Mvc.HttpPost, Microsoft.AspNetCore.Mvc.Route("user")]
		public System.Threading.Tasks.Task CreateUser([Microsoft.AspNetCore.Mvc.FromBody] User body)
		{

			HostX.Core.PetStoreControllerNS.RedirectCalls.HandleWithResponse("CreateUserAsync", new object[] { body });
			return _implementation.CreateUserAsync(body);
		}

	}

	[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.18.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0))")]
	public partial class ApiResponse
	{
		[Newtonsoft.Json.JsonProperty("code", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public int? Code { get; set; }

		[Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public string Type { get; set; }

		[Newtonsoft.Json.JsonProperty("message", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public string Message { get; set; }

	}

	[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.18.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0))")]
	public partial class Category
	{
		[Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public long? Id { get; set; }

		[Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public string Name { get; set; }

	}

	[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.18.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0))")]
	public partial class Pet
	{
		[Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public long? Id { get; set; }

		[Newtonsoft.Json.JsonProperty("category", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public Category Category { get; set; }

		[Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
		[System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
		public string Name { get; set; }

		[Newtonsoft.Json.JsonProperty("photoUrls", Required = Newtonsoft.Json.Required.Always)]
		[System.ComponentModel.DataAnnotations.Required]
		public System.Collections.Generic.List<string> PhotoUrls { get; set; } = new System.Collections.Generic.List<string>();

		[Newtonsoft.Json.JsonProperty("tags", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public System.Collections.Generic.List<Tag> Tags { get; set; }

		/// <summary>
		/// pet status in the store
		/// </summary>
		[Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
		public PetStatus? Status { get; set; }

	}

	[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.18.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0))")]
	public partial class Tag
	{
		[Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public long? Id { get; set; }

		[Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public string Name { get; set; }

	}

	[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.18.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0))")]
	public partial class Order
	{
		[Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public long? Id { get; set; }

		[Newtonsoft.Json.JsonProperty("petId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public long? PetId { get; set; }

		[Newtonsoft.Json.JsonProperty("quantity", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public int? Quantity { get; set; }

		[Newtonsoft.Json.JsonProperty("shipDate", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public System.DateTimeOffset? ShipDate { get; set; }

		/// <summary>
		/// Order Status
		/// </summary>
		[Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
		public OrderStatus? Status { get; set; }

		[Newtonsoft.Json.JsonProperty("complete", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public bool? Complete { get; set; }

	}

	[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.18.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0))")]
	public partial class User
	{
		[Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public long? Id { get; set; }

		[Newtonsoft.Json.JsonProperty("username", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public string Username { get; set; }

		[Newtonsoft.Json.JsonProperty("firstName", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public string FirstName { get; set; }

		[Newtonsoft.Json.JsonProperty("lastName", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public string LastName { get; set; }

		[Newtonsoft.Json.JsonProperty("email", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public string Email { get; set; }

		[Newtonsoft.Json.JsonProperty("password", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public string Password { get; set; }

		[Newtonsoft.Json.JsonProperty("phone", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public string Phone { get; set; }

		/// <summary>
		/// User Status
		/// </summary>
		[Newtonsoft.Json.JsonProperty("userStatus", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
		public int? UserStatus { get; set; }

	}

	[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.18.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0))")]
	public enum Anonymous
	{

		[System.Runtime.Serialization.EnumMember(Value = @"available")]
		Available = 0,

		[System.Runtime.Serialization.EnumMember(Value = @"pending")]
		Pending = 1,

		[System.Runtime.Serialization.EnumMember(Value = @"sold")]
		Sold = 2,

	}

	[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.18.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0))")]
	public enum PetStatus
	{

		[System.Runtime.Serialization.EnumMember(Value = @"available")]
		Available = 0,

		[System.Runtime.Serialization.EnumMember(Value = @"pending")]
		Pending = 1,

		[System.Runtime.Serialization.EnumMember(Value = @"sold")]
		Sold = 2,

	}

	[System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "13.18.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0))")]
	public enum OrderStatus
	{

		[System.Runtime.Serialization.EnumMember(Value = @"placed")]
		Placed = 0,

		[System.Runtime.Serialization.EnumMember(Value = @"approved")]
		Approved = 1,

		[System.Runtime.Serialization.EnumMember(Value = @"delivered")]
		Delivered = 2,

	}

	[System.CodeDom.Compiler.GeneratedCode("NSwag", "13.18.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0))")]
	public partial class FileParameter
	{
		public FileParameter(System.IO.Stream data)
			: this(data, null, null)
		{
		}

		public FileParameter(System.IO.Stream data, string fileName)
			: this(data, fileName, null)
		{
		}

		public FileParameter(System.IO.Stream data, string fileName, string contentType)
		{
			Data = data;
			FileName = fileName;
			ContentType = contentType;
		}

		public System.IO.Stream Data { get; private set; }

		public string FileName { get; private set; }

		public string ContentType { get; private set; }
	}


}

#pragma warning restore 1591
#pragma warning restore 1573
#pragma warning restore 472
#pragma warning restore 114
#pragma warning restore 108
#pragma warning restore 3016
#pragma warning restore 8603


namespace HostX.Core.PetStoreControllerNS
{

	public static class RedirectCalls
	{
		static System.Reflection.MethodInfo method = System.Reflection.Assembly.GetEntryAssembly().GetType("BeeRock.Core.Utils.RequestHandler").GetMethod("Handle");

		public static Dictionary<string, object> CreateParameter(string[] keys, object[] values)
		{
			var dict = new Dictionary<string, object>();
			for (int i = 0; i < keys.Length; i++) {
				dict.Add(keys[i], values[i]);
			}

			return dict;
		}

		public static string HandleWithResponse(string methodName, object[] parameters)
		{
			var p = new System.Collections.Generic.List<object>(parameters);
			p.Insert(0, methodName);
			var pp = new object[p.Count];
			for (int i = 0; i < p.Count; i++)
			{
				pp[i] = p[i];
			}

			method.Invoke(null, pp);

			return "";
		}
	}
}