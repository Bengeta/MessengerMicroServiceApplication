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



}