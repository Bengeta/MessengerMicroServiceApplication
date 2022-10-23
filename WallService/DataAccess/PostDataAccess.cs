using MongoDB.Driver;
using WallService.Models;

namespace WallService.DataAccess;

public class PostDataAccess
{
    private readonly IMongoCollection<Comment> comments;
    private readonly IMongoCollection<Post> posts;
    private  readonly  string _connectionString;
    
    public PostDataAccess(string connectionString)
    {
        _connectionString = connectionString;
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("WallDB");
        comments = database.GetCollection<Comment>("Comments");
        posts = database.GetCollection<Post>("Posts");
    }
    
    public async Task<List<Comment>> GetComments(int postId)
    {
        var Comments = await comments.Find(x => x.PostId == postId).ToListAsync();
        return Comments;
    }
    
    public async Task<List<Post>> GetUserPosts(int userId)
    {
        var Posts = await posts.Find(x => x.OwnerId == userId).ToListAsync();
        return Posts;
    }

    public async Task<List<Post>> GetPosts()
    {
        var Posts = await posts.Find(x => true).ToListAsync();
        return Posts;
    }
    
    public async Task<Post> GetPost(int postId)
    {
        var Post = await posts.Find(x => x.Id == postId).FirstOrDefaultAsync();
        return Post;
    }
    
    public async Task<ResultCode> CreatePost(AddPostRequest post)
    {
        try
        {
            var newPost = new Post
            {
                OwnerId = post.UserId,
                Text = post.Post.Text,
                CreateOn = DateTime.Now,
            };
            await posts.InsertOneAsync(newPost);
            return ResultCode.Success;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultCode.Failed;
        }
        
    }
    
    public async Task<Comment> CreateComment(Comment comment)
    {
        await comments.InsertOneAsync(comment);
        return comment;
    }
    
    public async Task<ResultCode> UpdatePost(UpdatePostRequest post)
    {
        try
        {
            var oldPost = await posts.Find(x => x.Id == post.PostId).FirstOrDefaultAsync();
            if (oldPost == null)
            {
                return ResultCode.Failed;
            }
            var newPost = new Post
            {
                Id = oldPost.Id,
                OwnerId = oldPost.OwnerId,
                Text = post.Post.Text,
                CreateOn = oldPost.CreateOn,
                Likes = oldPost.Likes,
                Reposts = oldPost.Reposts,
                Views = oldPost.Views,
                Comments = oldPost.Comments
            };
            await posts.ReplaceOneAsync(x => x.Id == newPost.Id, newPost);
            return ResultCode.Success;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultCode.Failed;
        }
    }
    
    public async Task<Comment> UpdateComment(Comment comment)
    {
        await comments.ReplaceOneAsync(x => x.Id == comment.Id, comment);
        return comment;
    }
    
    public async Task<ResultCode> DeletePost(int postId)
    {
        try
        {
            await posts.DeleteOneAsync(x => x.Id == postId);
            return ResultCode.Success;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultCode.Failed;
        }
    }
    
    public async Task DeleteComment(int commentId)
    {
        await comments.DeleteOneAsync(x => x.Id == commentId);
    }
    
    public async Task DeleteComments(int postId)
    {
        await comments.DeleteManyAsync(x => x.PostId == postId);
    }
    
    
}