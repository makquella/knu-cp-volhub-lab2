Vagrant.configure("2") do |config|
  config.vm.box_check_update = false

  # Вимикаємо перевірку Guest Additions, щоб не вимагати плагін vbguest
  config.vm.provider "virtualbox" do |vb|
    vb.check_guest_additions = false
  end

  # =========================
  # BaGet VM (локальне NuGet-сховище)
  # =========================
  config.vm.define "baget" do |baget|
    baget.vm.box = "ubuntu/jammy64"
    baget.vm.hostname = "baget"
    baget.vm.network "private_network", ip: "10.10.0.10"
    # Проброс порту, щоб BaGet був доступний з хоста
    baget.vm.network "forwarded_port", guest: 80, host: 5555, auto_correct: true

    baget.vm.provision "shell", inline: <<-SHELL
      set -e
      # Встановлення Docker
      apt-get update -y
      apt-get install -y ca-certificates curl gnupg
      install -m 0755 -d /etc/apt/keyrings
      curl -fsSL https://download.docker.com/linux/ubuntu/gpg | gpg --dearmor -o /etc/apt/keyrings/docker.gpg
      chmod a+r /etc/apt/keyrings/docker.gpg
      echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] \
        https://download.docker.com/linux/ubuntu $(. /etc/os-release && echo $VERSION_CODENAME) stable" \
        | tee /etc/apt/sources.list.d/docker.list > /dev/null
      apt-get update -y
      apt-get install -y docker-ce docker-ce-cli containerd.io

      # Запуск BaGet (in-memory, api-key: local-api-key)
      docker rm -f baget || true
      docker run -d --name baget -p 80:80 \
        -e "UseDatabase=InMemory" \
        -e "ApiKey=local-api-key" \
        loicsharma/baget:latest

      echo "BaGet запущено. Index: http://10.10.0.10/v3/index.json (host: http://localhost:5555/v3/index.json)"
    SHELL
  end

  # =========================
  # Ubuntu VM (демонстрація CLI)
  # =========================
  config.vm.define "ubuntu" do |ub|
    ub.vm.box = "ubuntu/jammy64"
    ub.vm.hostname = "ubuntu-app"
    ub.vm.network "private_network", ip: "10.10.0.11"
    ub.vm.network "forwarded_port", guest: 5000, host: 5001, auto_correct: true

    ub.vm.provision "shell", inline: <<-SHELL
      set -e

      # Оновлення сертифікатів та базових пакетів
      apt-get update -y
      apt-get install -y ca-certificates curl gnupg apt-transport-https

      # Додавання репозиторію Microsoft
      curl -sSL -o packages-microsoft-prod.deb https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
      dpkg -i packages-microsoft-prod.deb
      rm -f packages-microsoft-prod.deb

      apt-get update -y
      apt-get install -y dotnet-sdk-8.0

      # Підключення локального NuGet (BaGet)
      dotnet nuget list source | grep VolHubBaGet || dotnet nuget add source \
        --name VolHubBaGet --username any --password local-api-key --store-password-in-clear-text \
        http://10.10.0.10/v3/index.json

      # Встановлення глобального інструменту з BaGet
      export PATH="$HOME/.dotnet/tools:$PATH"
      dotnet tool install -g VolHub.Tool --version 1.0.0 --add-source VolHubBaGet \
        || dotnet tool update -g VolHub.Tool --version 1.0.0 --add-source VolHubBaGet

      # Запуск API на 0.0.0.0:5000 (на хості порт 5001)
      nohup $HOME/.dotnet/tools/volhub --urls http://0.0.0.0:5000 > volhub.log 2>&1 &
      echo "Ubuntu volhub: http://localhost:5001/swagger"
    SHELL
  end

  # =========================
  # Fedora VM (демонстрація CLI)
  # =========================
  config.vm.define "fedora" do |fd|
    fd.vm.box = "fedora/41-cloud-base"
    fd.vm.hostname = "fedora-app"
    fd.vm.network "private_network", ip: "10.10.0.12"
    fd.vm.network "forwarded_port", guest: 5000, host: 5002, auto_correct: true

    fd.vm.provision "shell", inline: <<-SHELL
      set -e
      # Встановлення .NET 8 через dnf (Fedora 41)
      sudo dnf -y install dotnet-sdk-8.0

      dotnet nuget list source | grep VolHubBaGet || dotnet nuget add source \
        --name VolHubBaGet --username any --password local-api-key --store-password-in-clear-text \
        http://10.10.0.10/v3/index.json

      export PATH="$HOME/.dotnet/tools:$PATH"
      dotnet tool install -g VolHub.Tool --version 1.0.0 --add-source VolHubBaGet \
        || dotnet tool update -g VolHub.Tool --version 1.0.0 --add-source VolHubBaGet

      # Запуск volhub на 0.0.0.0:5000 (хостовий порт 5002)
      nohup $HOME/.dotnet/tools/volhub --urls http://0.0.0.0:5000 > volhub.log 2>&1 &
      echo "Fedora volhub: http://localhost:5002/swagger"
    SHELL
  end
end
