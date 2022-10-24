using AutoMapper;
using Grpc.Core;
using WallService.DataAccess;
using WallService.Models;

namespace WallService.Services;

public class WallService : UserWall.UserWallBase 
{
    private readonly PostDataAccess _postAccess;
    private readonly IMapper _mapper;
    private readonly ILogger<GreeterService> _logger;

    public WallService(ILogger<GreeterService> logger,PostDataAccess postDataAccess, IMapper mapper)
    {
        _logger = logger;
        _postAccess = postDataAccess;
        _mapper = mapper;
    }
    
   public override async Task<GetAllUserPostsReply> GetAllUserPosts(GetAllUserPostsRequest request, ServerCallContext context)
    {
        var posts = await _postAccess.GetUserPosts(request.UserId);
        var reply = new GetAllUserPostsReply();
        reply.Post.AddRange(_mapper.Map<List<PostReply>>(posts));
        return reply;
    }
   
   public override async Task<AddPostReply> AddPost(AddPostRequest request, ServerCallContext context)
    {
        var reply = await _postAccess.CreatePost(request);
        return new AddPostReply(){ResultCode = reply};
    }
   
    public override async Task<DeletePostReply> DeletePost(DeletePostRequest request, ServerCallContext context)
     {
          var reply = await _postAccess.DeletePost(request.PostId);
          return new DeletePostReply(){ResultCode = reply};
     }

    public override async Task<UpdatePostReply> UpdatePost(UpdatePostRequest request, ServerCallContext context)
    {
        var reply = await _postAccess.UpdatePost(request);
        return new UpdatePostReply(){ResultCode = reply};
    }
    
    public override async Task<GetPostReply> GetPost(GetPostRequest request, ServerCallContext context)
    {
        var post = await _postAccess.GetPost(request.PostId);
        return _mapper.Map<GetPostReply>(post);
    }
    
    public override async Task<GetCommentsReply> GetComments(GetCommentsRequest request, ServerCallContext context)
    {
        var comments = await _postAccess.GetComments(request.PostId);
        var reply = new GetCommentsReply();
        reply.Comments.AddRange(_mapper.Map<List<Comments>>(comments));
        return reply;
    }

    public override async Task<AddCommentReply> AddComment(AddCommentRequest request, ServerCallContext context)
    {
        var reply = await _postAccess.CreateComment(request);
        return new AddCommentReply(){ResultCode = reply};
    }
    
    public override async Task<DeleteCommentReply> DeleteComment(DeleteCommentRequest request, ServerCallContext context)
    {
        var reply = await _postAccess.DeleteComment(request.CommentId);
        return new DeleteCommentReply(){ResultCode = reply};
    }
    
    public override async Task<UpdateCommentReply> UpdateComment(UpdateCommentRequest request, ServerCallContext context)
    {
        var reply = await _postAccess.UpdateComment(request);
        return new UpdateCommentReply(){ResultCode = reply};
    }
    
    public override async Task<CommentLikeReply> CommentLike(CommentLikeRequest request, ServerCallContext context)
    {
        var reply = await _postAccess.LikeComment(request.CommentId, request.UserId);
        return new CommentLikeReply(){ResultCode = reply};
    }
    
    public override async Task<PostLikeReply> PostLike(PostLikeRequest request, ServerCallContext context)
    {
        var reply = await _postAccess.LikePost(request.PostId, request.UserId);
        return new PostLikeReply(){ResultCode = reply};
    }




}