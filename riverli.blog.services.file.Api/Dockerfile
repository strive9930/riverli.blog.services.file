# =============================================================================
# Dockerfile — riverli-blog-file (文件服务)
# =============================================================================
# 构建命令（在解决方案根目录执行）：
#   docker build -t riverli-blog-file -f riverli.blog.services.file.Api/Dockerfile .
# 注意：本服务依赖 Application / Infrastructure / Domain 三层类库，
#       必须提前复制全部 .csproj 文件，否则 dotnet restore 会因缺少引用而失败
# =============================================================================

# ─── 阶段 1：构建环境 ───
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# ⚠️ 关键步骤：复制所有层项目的 .csproj 文件
# Api → Application → Domain
# Api → Infrastructure → Application → Domain
COPY ["riverli.blog.services.file.Api/riverli.blog.services.file.Api.csproj", "riverli.blog.services.file.Api/"]
COPY ["riverli.blog.services.file.Application/riverli.blog.services.file.Application.csproj", "riverli.blog.services.file.Application/"]
COPY ["riverli.blog.services.file.Infrastructure/riverli.blog.services.file.Infrastructure.csproj", "riverli.blog.services.file.Infrastructure/"]
COPY ["riverli.blog.services.file.Domain/riverli.blog.services.file.Domain.csproj", "riverli.blog.services.file.Domain/"]

# 恢复全部 NuGet 依赖（包括 RiverLi.Blog.Infrastructure.Shared、MediatR、EF Core、Pomelo.MySql 等）
RUN dotnet restore "riverli.blog.services.file.Api/riverli.blog.services.file.Api.csproj"

# 复制所有层项目源码
COPY . .

# 发布 Api 项目
WORKDIR "/src/riverli.blog.services.file.Api"
RUN dotnet publish "riverli.blog.services.file.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ─── 阶段 2：极简运行环境 ───
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "riverli.blog.services.file.Api.dll"]
