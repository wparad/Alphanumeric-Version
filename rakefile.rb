#!/usr/bin/ruby
require 'bundler/setup'
require 'fileutils'
require 'rake'
require 'rake/clean'
require 'fileutils'
require 'tmpdir'
require 'travis-build-tools'

PWD = File.dirname(__FILE__)
OUTPUT_DIR = File.join(PWD, 'output')
SOLUTION_SLN = Dir['./*.sln'].first
GIT_REPOSITORY = %x[git config --get remote.origin.url].split('://')[1]
VERSION = TravisBuildTools::Build::VERSION.to_s

#Environment variables: http://docs.travis-ci.com/user/environment-variables/
#### TASKS ####
  task :default => [:build]

  task :after_build => [:display_repository, :publish_git_tag, :merge_downstream]

directory OUTPUT_DIR

task :build => [OUTPUT_DIR] do
  raise 'Nuget restore failed' if !system("nuget restore #{SOLUTION_SLN}")
  raise 'Build failed' if !system("xbuild /p:Configuration=Release #{SOLUTION_SLN}")
  Dir.mktmpdir do |tmp|
    nuspec = File.join(tmp, 'AlphanumericVersion.nuspec')
    File.write(nuspec, "<?xml version=\"1.0\" encoding=\"utf-8\"?>
<package xmlns=\"http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd\">
  <metadata>
    <id>AlphanumericVersion</id>
    <title>AlphanumericVersion</title>
    <version>#{VERSION}</version>
    <authors>Warren Parad</authors>
    <owners>Warren Parad</owners>
    <projectUrl>https://#{GIT_REPOSITORY}</projectUrl>
    <description>AlphanumericVersion for C#</description>
  </metadata>
  <files>
    <file src=\"src/AlphanumericVersion/obj/Release/**/*.*\" target=\"lib/net40\" />
  </files>
</package>
")
    raise 'Nuget packing failed' if !system("nuget pack '#{nuspec}' -BasePath #{PWD} -OutputDirectory #{OUTPUT_DIR} -Verbosity detailed")
  end
end

BUILDER = TravisBuildTools::Builder.new(ENV['GIT_TAG_PUSHER'] || ENV['USER'])
task :publish_git_tag do
  BUILDER.publish_git_tag(TravisBuildTools::Build::VERSION.to_s)
end

task :merge_downstream do
  BUILDER.merge_downstream('release/', 'master')
end

task :display_repository do
  puts Dir.glob(File.join(PWD, '**', '*'), File::FNM_DOTMATCH).select{|f| !f.match(/\/(\.git|vendor|bundle)\//)}
end
