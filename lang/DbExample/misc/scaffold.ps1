# �����̃e�[�u����`����DBContext, Entity�𐶐�

# dotnet ef dbcontext scaffold�w���v
#   https://learn.microsoft.com/ja-jp/ef/core/cli/dotnet

<#
  --data-annotations: 
    �e�[�u�����E�J����������Entity�̃A�m�e�[�V�����Ƃ��ĕt�^
  --no-pluralize:
    DBContext��DbSet�v���p�e�B��(�e�[�u����)�𕡐��`�̖��O�ɂ��Ȃ�
    (Entity�Ɩ��O���قȂ�ƍ�������̂�)
  --no-onconfiguring:
    OnConfiguring()���\�b�h�̏o�͂�}������B
    partial��`�����Ǝ��N���X����OnConfiguring()���g�p���邽�߁B
#>

cd ..

$constr = "Server=localhost;Initial Catalog=DbExample;Integrated Security=true;TrustServerCertificate=true"
dotnet ef dbcontext scaffold "${constr}" `
	Microsoft.EntityFrameworkCore.SqlServer `
	--context AppDbContext --context-dir Context `
	--output-dir Entity `
	--data-annotations --no-pluralize --no-onconfiguring --force

pause
