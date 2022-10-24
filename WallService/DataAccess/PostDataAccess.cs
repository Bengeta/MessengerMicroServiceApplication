using MongoDB.Driver;
using WallService.Models;

namespace WallService.DataAccess;

public class PostDataAccess
{
    private readonly IMongoCollection<Comment> comments;
    private readonly IMongoCollection<Post> posts;
    private readonly string _connectionString;

    public PostDataAccess(string connectionString)
    {
        _connectionString = connectionString;
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("WallDB");
        comments = database.GetCollection<Comment>("Comments");
        posts = database.GetCollection<Post>("Posts");
    }

    public async Task<List<Comments>> GetComments(int postId)
    {
        var Comments = await comments.Find(x => x.PostId == postId).ToListAsync();
        var result = Comments.Select(x => new Comments
        {
            Id = x.Id,
            PostId = x.PostId,
            Text = x.Text,
            CreateOn = (ulong) ((x.CreateOn.Ticks - 621355968000000000) / 10000000),
            Likes = x.Likes.Count,
            UserId = x.UserId
        }).ToList();
        return result;
    }

    public async Task<List<Post>> GetUserPosts(int userId)
    {
        var Posts = await posts.Find(x => x.OwnerId == userId).ToListAsync();
        return Posts;
    }

    public async Task<List<Post>> GetPosts()
    {
        try
        {
            var Posts = await posts.Find(x => true).ToListAsync();
            return Posts;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<Post> GetPost(int postId)
    {
        try
        {
            var Post = await posts.Find(x => x.Id == postId).FirstOrDefaultAsync();
            return Post;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
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

    public async Task<ResultCode> CreateComment(AddCommentRequest comment)
    {
        try
        {
            var newComment = new Comment
            {
                PostId = comment.PostId,
                Text = comment.Comment.Text,
                CreateOn = DateTime.Now,
                UserId = comment.UserId
            };
            await comments.InsertOneAsync(newComment);
            return ResultCode.Success;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultCode.Failed;
        }
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
            var result = await posts.ReplaceOneAsync(x => x.Id == newPost.Id, newPost);
            if (result.ModifiedCount == 1)
                return ResultCode.Success;
            else
                return ResultCode.Success;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultCode.Failed;
        }
    }

    public async Task<ResultCode> UpdateComment(UpdateCommentRequest comment)
    {
        try
        {
            var oldComment = await comments.Find(x => x.Id == comment.CommentId).FirstOrDefaultAsync();
            if (oldComment == null)
                return ResultCode.Failed;

            var newComment = new Comment
            {
                Id = comment.CommentId,
                PostId = comment.PostId,
                Text = comment.Comment.Text,
                CreateOn = oldComment.CreateOn,
                Likes = oldComment.Likes,
                UserId = comment.UserId
            };

            var result = await comments.ReplaceOneAsync(x => x.Id == newComment.Id, newComment);
            if (result.ModifiedCount == 0)
                return ResultCode.Failed;
            return ResultCode.Success;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultCode.Failed;
        }
    }

    public async Task<ResultCode> DeletePost(int postId)
    {
        try
        {
            var result = await posts.DeleteOneAsync(x => x.Id == postId);
            if (result.DeletedCount == 0)
                return ResultCode.Failed;
            return ResultCode.Success;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultCode.Failed;
        }
    }

    public async Task<ResultCode> DeleteComment(int commentId)
    {
        try
        {
            var result = await comments.DeleteManyAsync(x => x.Id == commentId);
            if (result.DeletedCount == 0)
                return ResultCode.Failed;
            return ResultCode.Success;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultCode.Failed;
        }
    }

    public async Task DeleteComments(int postId)
    {
        try
        {
            await comments.DeleteManyAsync(x => x.PostId == postId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultCode> LikePost(int postId, int userId)
    {
        try
        {
            var post = await posts.Find(x => x.Id == postId).FirstOrDefaultAsync();
            if (post == null)
                return ResultCode.Failed;
            if (post.Likes.Contains(userId))
                post.Likes.Remove(userId);
            else
                post.Likes.Add(userId);
            await posts.ReplaceOneAsync(x => x.Id == post.Id, post);
            return ResultCode.Success;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultCode.Failed;
        }
    }

    public async Task<ResultCode> LikeComment(int commentId, int userId)
    {
        try
        {
            var comment = await comments.Find(x => x.Id == commentId).FirstOrDefaultAsync();
            if (comment == null)
                return ResultCode.Failed;
            if (comment.Likes.Contains(userId))
                comment.Likes.Remove(userId);
            else
                comment.Likes.Add(userId);
            await comments.ReplaceOneAsync(x => x.Id == comment.Id, comment);
            return ResultCode.Success;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultCode.Failed;
        }
    }
}